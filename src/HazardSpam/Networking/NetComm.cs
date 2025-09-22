using System.Linq;
using ConsoleTools;
using HazardSpam.Hazards;
using HazardSpam.Helpers;
using HazardSpam.Types;
using NetGameState.Level;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Networking;

public class NetComm : MonoBehaviourPun
{
    public static NetComm Instance { get; private set; } = null!;

    private void Awake()
    {
        if (!ReferenceEquals(Instance, null))
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ============================================================================
    //  Spawner Creation
    // ============================================================================
    internal void CreateMultipleSpawnersNetwork(SpawnIdentifierNet[] spawnerData)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int[] zones = spawnerData.Select(s => (int)s.Zone).ToArray();
        int[] areas = spawnerData.Select(s => (int)s.Area).ToArray();
        int[] types = spawnerData.Select(s => (int)s.Type).ToArray();
        int[] viewIDs = spawnerData.Select(s => s.ViewID).ToArray();
        
        Plugin.Log.LogColor(string.Join(", ", viewIDs));
        
        Plugin.Log.LogColor($"[Network] Sending 'RPC_CreateMultipleSpawners' for {spawnerData.Length} spawners");
        photonView.RPC("RPC_CreateMultipleSpawners", RpcTarget.All, zones, areas, types, viewIDs);
    }

    [PunRPC]
    public void RPC_CreateMultipleSpawners(int[] zones, int[] areas, int[] types, int[] viewIDs)
    {
        Plugin.Log.LogColor($"[Network] Receiving 'RPC_CreateMultipleSpawners' for {zones.Length} spawners");
        for (int i = 0; i < zones.Length; i++) 
        {
            HazardManager.CreateSpawner(
                (Zone)zones[i],
                (SubZoneArea)areas[i],
                (HazardType)types[i],
                viewIDs[i]);
        }
    }

    internal void CreateSpawnerNetwork(SpawnIdentifierNet spawnIdentifierNet)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        string id = Helper.GenSpawnID(spawnIdentifierNet.Zone, spawnIdentifierNet.Area, spawnIdentifierNet.Type);
        Plugin.Log.LogColor($"[Network] Sending 'RPC_CreateSpawner' for {id}");
        photonView.RPC("RPC_CreateSpawner", RpcTarget.All,
            (int)spawnIdentifierNet.Zone,
            (int)spawnIdentifierNet.Area,
            (int)spawnIdentifierNet.Type,
            spawnIdentifierNet.ViewID);
    }

    [PunRPC]
    public void RPC_CreateSpawner(int zone, int area, int type, int viewID)
    {
        Zone z = (Zone)zone;
        SubZoneArea a = (SubZoneArea)area;
        HazardType t = (HazardType)type;
        Plugin.Log.LogColor($"[Network] Receiving 'RPC_CreateSpawner' for {Helper.GenSpawnID(z, a, t)}");
        HazardManager.CreateSpawner(z, a, t, viewID);
    }
    
    // ============================================================================
    //  Spawner Removal
    // ============================================================================
    internal void RemoveSpawnersNetwork(Zone zone)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        Plugin.Log.LogColor($"[Network] Sending 'RPC_RemoveSpawners' for {zone.ToString()}");
        photonView.RPC("RPC_RemoveSpawners", RpcTarget.All, (int)zone);
    }

    [PunRPC]
    public void RPC_RemoveSpawners(int zone)
    {
        Plugin.Log.LogColor($"[Network] Receiving 'RPC_RemoveSpawners' for {((Zone)zone).ToString()}");
        HazardManager.RemoveSpawnersInZone((Zone)zone);
    }

    // ============================================================================
    //  Spawn hazards
    // ============================================================================
    internal void SpawnHazardsNetwork(Zone zone, SubZoneArea area, HazardType type, Vector3[] positions, Quaternion[] rotations)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (positions.Length == 0)
            return;
        
        string id = Helper.GenSpawnID(zone, area, type);
        Plugin.Log.LogColor($"[Network] Sending 'RPC_SpawnHazards' for {id} with {positions.Length} spawns");
        photonView.RPC("RPC_SpawnHazards", RpcTarget.All, zone, area, type, positions, rotations);
    }

    [PunRPC]
    public void RPC_SpawnHazards(Zone zone, SubZoneArea area, HazardType type, Vector3[] positions, Quaternion[] rotations)
    {
        string id = Helper.GenSpawnID(zone, area, type);
        Plugin.Log.LogColor($"[Network] Receiving 'RPC_SpawnHazards' for {id} with {positions.Length} spawns");

        StartCoroutine(HazardManager.SpawnPropsRoutine(zone, area, type, positions, rotations));
    }
    
    
    // ============================================================================
    //  Helpers
    // ============================================================================
    
    internal GameObject? InstantiateHazard(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(prefab, position, rotation, parent);
    }
}