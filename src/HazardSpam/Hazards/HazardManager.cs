using System.Collections;
using System.Collections.Generic;
using ConsoleTools;
using HazardSpam.Helpers;
using HazardSpam.Networking;
using HazardSpam.Types;
using NetGameState.Level;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Hazards;

public static class HazardManager
{
    internal static readonly Dictionary<(Zone, SubZoneArea, HazardType), GameObject> ActiveSpawners = [];


    internal static void Reset()
    {
        ActiveSpawners.Clear();
    }

    internal static void RemoveSpawnersInZone(Zone zoneToClear)
    {
        var toRemove = new List<(Zone, SubZoneArea, HazardType)>();
        
        foreach (var (zone, subZoneArea, spawnType) in ActiveSpawners.Keys)
        {
            if (zone == zoneToClear)
            {
                toRemove.Add((zone, subZoneArea, spawnType));
            }
        }

        foreach (var key in toRemove)
        {
            Object.Destroy(ActiveSpawners[key]);
            ActiveSpawners.Remove(key);
            Plugin.Log.LogColor($"Removed spawner '{Helper.GenSpawnID(key.Item1, key.Item2, key.Item3)}'");
        }
    }

    private static bool DoesSpawnerExist(Zone zone, SubZoneArea area, HazardType hazardType)
    {
        return ActiveSpawners.ContainsKey((zone, area, hazardType));
    }

    private static int GenSpawnerViewID(HazardType hazardType)
    {
        if (!PhotonNetwork.IsMasterClient)
            return -1;
        
        return hazardType switch
        {
            //case SpawnType.NiceThorn:
            //case SpawnType.BigThorn:
            HazardType.Jelly or
            HazardType.Thorn or
            HazardType.BigThorn or
            HazardType.ExploSpore or
            HazardType.PoisonSpore or
            HazardType.Geyser or
            HazardType.FlashPlant => PhotonNetwork.AllocateViewID(true),
            _ => -1
        };
    }

    internal static bool TryCreateSpawnerData(Zone zone, SubZoneArea area, HazardType hazardType, out SpawnIdentifierNet spawnIdentifierNet)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            spawnIdentifierNet = new SpawnIdentifierNet();
            return false;
        }
        
        if (DoesSpawnerExist(zone, area, hazardType))
        {
            Plugin.Log.LogColorW($"Master can't create spawner '{Helper.GenSpawnID(zone, area, hazardType)}' as it already exists.");
            spawnIdentifierNet = new SpawnIdentifierNet();
            return false;
        }
        
        spawnIdentifierNet = new SpawnIdentifierNet(zone, area, hazardType, GenSpawnerViewID(hazardType));
        return true;
    }

    private static string GenSpawnerName(SubZoneArea area, HazardType type)
    {
        return $"{area.ToString()}_{type.ToString()}";
    }

    internal static void CreateSpawner(Zone zone, SubZoneArea area, HazardType hazardType, int viewID)
    {
        if (!HazardTemplateManager.PropPrefabs.TryGetValue(hazardType, out var propSpawnerPrefab))
        {
            Plugin.Log.LogColorW($"Could not get prefab for '{hazardType.ToString()}'");
            return;
        }

        if (!HazardTemplateManager.PropSpawners.TryGetValue((zone, area), out var propSpawnerArea))
        {
            Plugin.Log.LogColorW($"Could not get propSpawner for '{zone.ToString()}/{area.ToString()}'");
            return;
        }

        if (!HazardTemplateManager.BiomeSpawnerRoots.TryGetValue(zone, out var root))
        {
            Plugin.Log.LogColorW($"Could not get root for '{zone.ToString()}'");
            return;
        }
        
        if (DoesSpawnerExist(zone, area, hazardType))
        {
            Plugin.Log.LogColorW($"Spawner '{Helper.GenSpawnID(zone, area, hazardType)}' already exists.");
            return;
        }

        // Create spawner
        var spawnerGo = new GameObject(GenSpawnerName(area, hazardType));
        spawnerGo.transform.SetParent(root.transform, worldPositionStays: false);
        
        // Set position
        spawnerGo.transform.position = propSpawnerArea.transform.position;
        spawnerGo.transform.rotation = propSpawnerArea.transform.rotation;
        spawnerGo.transform.localScale = propSpawnerArea.transform.localScale;
        
        // Create PhotonView
        if (viewID != -1)
        {
            var pv = spawnerGo.AddComponent<PhotonView>();
            pv.ViewID = viewID;
        }
        
        // Create PropSpawner
        var propSpawner = spawnerGo.AddComponent<PropSpawner>();
        
        // Configure PropSpawner
        propSpawner.area = propSpawnerArea.area;
        propSpawner.rayDirectionOffset = propSpawnerArea.rayDirectionOffset;
        propSpawner.rayLength = propSpawnerArea.rayLength;
        propSpawner.raycastPosition = propSpawnerArea.raycastPosition;
        propSpawner.nrOfSpawns = 1;
        propSpawner.chanceToUseSpawner = 1f;
        propSpawner.syncTransforms = false;
        propSpawner.layerType = propSpawnerArea.layerType;
        propSpawner.props = [propSpawnerPrefab.props[0]];
        
        // Configure this if needed...
        propSpawner.modifiers = new List<PropSpawnerMod>(propSpawnerPrefab.modifiers);
        propSpawner.constraints = new List<PropSpawnerConstraint>(propSpawnerPrefab.constraints);
        propSpawner.postConstraints = new List<PropSpawnerConstraintPost>(propSpawnerPrefab.postConstraints);
        
        // Add Trigger Relay
        var triggerRelay = spawnerGo.AddComponent<TriggerRelay>();
        
        // Add to registry
        ActiveSpawners[(zone, area, hazardType)] = spawnerGo;
    }
    
    internal static IEnumerator SpawnPropsRoutine(Zone zone, SubZoneArea area, HazardType type, Vector3[] positions, Quaternion[] rotations)
    {
        string id = Helper.GenSpawnID(zone, area, type);
        
        if (!HazardTemplateManager.PropPrefabs.TryGetValue(type, out var propSpawnerPrefab))
        {
            Plugin.Log.LogColorW($"Could not get prefab for '{type.ToString()}'");
            yield break;
        }

        if (!ActiveSpawners.TryGetValue((zone, area, type), out var spawnerGo))
        {
            Plugin.Log.LogColorW($"Can't find spawner: {id}");
            yield break;
        }

        for (int i = 0; i < positions.Length; i++)
        {
            NetComm.Instance.InstantiateHazard(propSpawnerPrefab.props[0], positions[i], rotations[i], spawnerGo.transform);
            
            if (i % 20 == 0)
                yield return null;
        }
    }
}