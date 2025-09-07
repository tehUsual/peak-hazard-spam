using HazardSpam.Level;
using HazardSpam.Level.Biomes;
using HazardSpam.Level.Spawner;
using Photon.Pun;
using HazardSpam.Spawning;
using HazardSpam.Types;
using UnityEngine;

namespace HazardSpam.Networking;

public class SpawnerNetwork : MonoBehaviourPun
{
    public static SpawnerNetwork Instance { get; private set; } = null!;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void SpawnPropsNetwork(SpawnerData spawnData, Vector3[] positions, Quaternion[] rotations, float[] scaleGain)
    {
        if (positions.Length == 0) return;
        
        Plugin.Log.LogInfo($"[Network] Sending 'RPC_SpawnProps' for {positions.Length} at {spawnData.BiomeType.ToString()}/{spawnData.Area.ToString()}/{spawnData.SpawnType.ToString()}");
        
        photonView.RPC(
            "RPC_SpawnProps",
            RpcTarget.All,
            spawnData.BiomeType, spawnData.Area, spawnData.SpawnType,
            positions, rotations
        );
    }
    
    [PunRPC]
    public void RPC_SpawnProps(Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType,
        Vector3[] positions, Quaternion[] rotations)
    {
        Plugin.Log.LogInfo($"[Network] Received 'RPC_SpawnProps' for {positions.Length} at {biomeType.ToString()}/{area.ToString()}/{spawnType.ToString()}");
        
        BiomeInfo? biomeInfo = LevelManager.GetBiomeInfo(biomeType);
        if (biomeInfo == null) return;
        
        foreach (var spawner in biomeInfo.Spawners)
        {
            if (spawner.Area == area && spawner.SpawnType == spawnType)
            {
                string areaName = $"{spawner.BiomeType.ToString()}/{spawner.Area.ToString()}"; 
                SpawnPropsInWorld(spawner, positions, rotations, areaName);
                return;
            }
        }
    }

    private void SpawnPropsInWorld(SpawnerData spawnerData, Vector3[] positions, Quaternion[] rotations, string area)
    {
        int spawns = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            var go = Instantiate(spawnerData.Prefab, positions[i], rotations[i], spawnerData.PropSpawner.transform);
            if (go != null)
            {
                go.AddComponent<SpawnedPropHS>();
                spawnerData.AddSpawnInfo(positions[i], rotations[i]);
                spawns++;
            }
        }
        Plugin.Log.LogInfo($"Spawned {spawns}/{positions.Length} '{spawnerData.Prefab.name}' in {area}");
    }

    
    public void ClearPropsNetwork(SpawnerData spawnData)
    {
        Plugin.Log.LogInfo($"[Network] Sending 'RPC_ClearProps' {spawnData.BiomeType.ToString()}/{spawnData.Area.ToString()}/{spawnData.SpawnType.ToString()}");
        
        photonView.RPC(
            "RPC_ClearProps",
            RpcTarget.All,
            spawnData.BiomeType,
            spawnData.Area,
            spawnData.SpawnType);
    }

    [PunRPC]
    public void RPC_ClearProps(Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType)
    {
        Plugin.Log.LogInfo($"[Network] Received 'RPC_ClearProps' at {biomeType.ToString()}/{area.ToString()}/{spawnType.ToString()}");
        
        BiomeInfo? biomeInfo = LevelManager.GetBiomeInfo(biomeType);
        if (biomeInfo == null) return;
    
        foreach (var spawner in biomeInfo.Spawners)
        {
            if (spawner.Area == area && spawner.SpawnType == spawnType)
            {
                string areaName = $"{spawner.BiomeType.ToString()}/{spawner.Area.ToString()}";

                int removed = ClearPropsInWorld(spawner, areaName);
                Plugin.Log.LogInfo($"Cleared {removed} '{spawnType.ToString()}' from {areaName}");
                return;
            }
        }
    }
    
    private int ClearPropsInWorld(SpawnerData spawnerData, string area)
    {
        int removed = 0;
        for (int i = spawnerData.PropSpawner.transform.childCount - 1; i >= 0; i--)
        {
            var go = spawnerData.PropSpawner.transform.GetChild(i).gameObject;
            if (go.GetComponent<SpawnedPropHS>() != null)
            {
                Destroy(go);
                removed++;
            }
        }
        spawnerData.Clear();
        
        return removed;
    }
}