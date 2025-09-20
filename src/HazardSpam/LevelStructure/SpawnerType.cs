using System.Collections;
using System.Collections.Generic;
using ConsoleTools;
using HazardSpam.Networking;
using HazardSpam.Spawning;
using HazardSpam.Types;
using NetGameState.Level;
using NetGameState.Level.Helpers;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.LevelStructure;

public class SpawnerType(string id, Zone zone, SubZoneArea area, SpawnType spawnType, string path, int amountToSpawn = 0)
{
    public string ID { get; private set; } = id;
    public Zone Zone { get; private set; } = zone;
    public SubZoneArea Area { get; private set; } = area;
    public SpawnType SpawnType { get; private set; } = spawnType;
    public string Path { get; private set; } = path;
    public int AmountToSpawn { get; private set; } = amountToSpawn;
    
    public bool IsValid { get; private set; }
    public bool IsActive { get; private set; }
    
    public Transform? SpawnerTransform { get; private set; }
    public PropSpawner? PropSpawner { get; private set; }
    public GameObject? Prefab { get; private set; }

    public List<GameObject> SpawnedObjects { get; private set; } = [];


    public void LoadSpawner(SubZone currentSubZone)
    {
        string subZoneString = SegmentMapper.SubZoneToExactString(Zone, currentSubZone);
        string newPath = Path.Replace("*", subZoneString);
        GameObject? obj = GameObject.Find(newPath);
        if (ReferenceEquals(obj, null))
        {
            Plugin.Log.LogColorW($"Could not find spawner '{ID}' at {newPath}");
            return;
        }

        SpawnerTransform = obj.transform;
        PropSpawner = SpawnerTransform?.GetComponent<PropSpawner>();
        Prefab = PropSpawner?.props[0];

        IsValid = (!ReferenceEquals(SpawnerTransform, null) &&
                   !ReferenceEquals(PropSpawner, null) &&
                   !ReferenceEquals(Prefab, null));
    }

    public IEnumerator InstantiateChildren(Vector3[] positions, Quaternion[] rotations)
    {
        if (!IsValid && !IsActive)
            yield break;
        
        int spawns = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            var go = SpawnerNetwork.Instance.InstantiatePrefab(Prefab!, positions[i], rotations[i], SpawnerTransform!);
            if (!ReferenceEquals(go, null))
            {
                go.AddComponent<SpawnedPropHS>();
                spawns++;
                
                // to support reconnecting/late joining players
                if (PhotonNetwork.IsMasterClient)
                    SpawnedObjects.Add(go);
            }
            
            if (i % 10 == 0 && i != 0)
                yield return null;
        }
        
        Plugin.Log.LogInfo($"Spawned {spawns}/{positions.Length} '{Prefab!.name}' in {Zone}/{Area}");
    }

    public void DeleteSpawns()
    {
        if (!IsValid)
            return;

        for (int i = SpawnerTransform!.childCount - 1; i >= 0; i--)
        {
            var go = SpawnerTransform!.GetChild(i).gameObject;
            if (!ReferenceEquals(go.GetComponent<SpawnedPropHS>(), null))
            {
                SpawnerNetwork.Instance.DestroyGameObject(go);
            }
        }
        
        SpawnedObjects.Clear();
        IsActive = false;
    }
}