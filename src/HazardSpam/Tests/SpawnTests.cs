using System.Collections;
using System.Collections.Generic;
using ConsoleTools;
using HazardSpam.Hazards;
using HazardSpam.Helpers;
using HazardSpam.Networking;
using HazardSpam.Spawning;
using HazardSpam.Types;
using NetGameState.Level;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Tests;

public static class SpawnTests
{
    internal static void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        // CTRL
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
                CreateSpawner(Zone.Shore, SubZoneArea.Plateau, HazardType.ExploSpore);
            if (Input.GetKeyDown(KeyCode.Keypad2))
                CreateSpawner(Zone.Shore, SubZoneArea.Plateau, HazardType.Thorn);
            if (Input.GetKeyDown(KeyCode.Keypad3))
                CreateSpawner(Zone.Shore, SubZoneArea.Wall, HazardType.PoisonSpore);

            if (Input.GetKeyDown(KeyCode.Keypad4))
                CreateSpawners(Zone.Tropics, SubZoneArea.Wall, [HazardType.Beehive, HazardType.PoisonSpore, HazardType.CactusBig]);
            if (Input.GetKeyDown(KeyCode.Keypad5))
                CreateSpawners(Zone.Alpine, SubZoneArea.Wall, [HazardType.Geyser, HazardType.Urchin, HazardType.LavaRiver]);
            if (Input.GetKeyDown(KeyCode.Keypad6))
                CreateSpawners(Zone.Caldera, SubZoneArea.Plateau, [HazardType.PoisonSpore, HazardType.Jelly, HazardType.PoisonIvy, HazardType.Dynamite]);
        }
        
        // SHIFT
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
                SpawnHazards(Zone.Shore, SubZoneArea.Plateau, HazardType.ExploSpore);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                SpawnHazards(Zone.Shore, SubZoneArea.Plateau, HazardType.Thorn);
            if (Input.GetKeyDown(KeyCode.Alpha0))
                SpawnHazards(Zone.Shore, SubZoneArea.Wall, HazardType.PoisonSpore);
        }
    }

    private static void SpawnHazards(Zone zone, SubZoneArea area, HazardType type)
    {
        if (!HazardManager.ActiveSpawners.TryGetValue((zone, area, type), out var spawnerGo))
        {
            Plugin.Log.LogColorW($"Could not get spawner from Active Spawners {zone.ToString()}/{area.ToString()}/{type.ToString()}");
            return;
        }
        
        var propSpawner = spawnerGo.GetComponent<PropSpawner>();
        var (positions, rotations) = SpawnerLogic.GenerateSpawnPoints(propSpawner, 1000);
        Plugin.Log.LogColor($"Generated {positions.Length} positions out of {1000}");
        if (positions.Length == 0)
            return;
                
        var spawnId = Helper.GenSpawnID(zone, area, type);
        for (int i = 0; i < positions.Length; i++)
        {
            var propGo = NetComm.Instance.InstantiateHazard(propSpawner.props[0], positions[i], rotations[i], spawnerGo.transform);
        }
    }

    private static void CreateSpawner(Zone zone, SubZoneArea area, HazardType type)
    {
        if (HazardManager.TryCreateSpawnerData(zone, area, type, out var spawnerDataNet))
        {
            NetComm.Instance.CreateSpawnerNetwork(spawnerDataNet);    
        }
    }

    private static void CreateSpawners(Zone zone, SubZoneArea area, HazardType[] types)
    {
        List<SpawnIdentifierNet> spawnerData = [];
        foreach (var spawnType in types)
        {
            if (HazardManager.TryCreateSpawnerData(zone, area, spawnType, out var spawnerDataNet))
                spawnerData.Add(spawnerDataNet);
        }
        
        if (spawnerData.Count > 0)
            NetComm.Instance.CreateMultipleSpawnersNetwork(spawnerData.ToArray());
    }


    internal static IEnumerator InitializeRunTypeTest()
    {
        HazardTemplateManager.LoadSceneData();
        yield return new WaitForSeconds(1f);
        
        yield return CreateSpawnerTypes();
        yield return new WaitForSeconds(2f);
        
        yield return LoadZone(Zone.Shore);
    }

    internal static IEnumerator InitializeRunSegmentTest()
    {
        HazardTemplateManager.LoadSceneData();
        CreateSpawnerSegments();
        yield return LoadZone(Zone.Shore);
    }

    internal static void CreateSpawnerSegments()
    {
        List<SpawnIdentifier> spawnerIds =
        [
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Jelly),
            new (Zone.Shore, SubZoneArea.Wall, HazardType.Jelly),
            
            new (Zone.Tropics, SubZoneArea.Plateau, HazardType.Jelly),
            new (Zone.Tropics, SubZoneArea.Wall, HazardType.Jelly),
            
            new (Zone.Alpine, SubZoneArea.Plateau, HazardType.Jelly),
            new (Zone.Alpine, SubZoneArea.Wall, HazardType.Jelly),
            
            new (Zone.Mesa, SubZoneArea.Plateau, HazardType.Jelly),
            new (Zone.Mesa, SubZoneArea.Wall, HazardType.Jelly),
            
            new (Zone.Caldera, SubZoneArea.Plateau, HazardType.Jelly),
        ];
        
        List<SpawnIdentifierNet> spawnerNetIds = new List<SpawnIdentifierNet>();
        foreach (var spawnerId in spawnerIds)
        {
            var (zone, area, spawnType) = spawnerId;
            if (HazardManager.TryCreateSpawnerData(zone, area, spawnType, out var spawnerDataNet))
            {
                spawnerNetIds.Add(spawnerDataNet);
            }
        }
        
        if (spawnerNetIds.Count > 0)
            NetComm.Instance.CreateMultipleSpawnersNetwork(spawnerNetIds.ToArray());
    }

    internal static IEnumerator CreateSpawnerTypes()
    {
        List<SpawnIdentifier> spawnerIds =
        [
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Urchin),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Jelly),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.PoisonIvy),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.PoisonSpore),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Thorn),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.BigThorn),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Beehive),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Geyser),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.FlashPlant),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.CactusBall),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Cactus),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.CactusBig),
            new (Zone.Shore, SubZoneArea.Plateau, HazardType.Dynamite),

            new (Zone.Shore, SubZoneArea.Wall, HazardType.LavaRiver),
        ];


        List<SpawnIdentifierNet> spawnerNetIds = new List<SpawnIdentifierNet>();
        foreach (var spawnerId in spawnerIds)
        {
            var (zone, area, spawnType) = spawnerId;
            if (HazardManager.TryCreateSpawnerData(zone, area, spawnType, out var spawnerDataNet))
            {
                spawnerNetIds.Add(spawnerDataNet);
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        if (spawnerNetIds.Count > 0)
            NetComm.Instance.CreateMultipleSpawnersNetwork(spawnerNetIds.ToArray());
    }


    internal static IEnumerator LoadZone(Zone zoneToLoad)
    {
        Plugin.Log.LogColor($"Loading zone {zoneToLoad}");

        if (!PhotonNetwork.IsMasterClient)
        {
            Plugin.Log.LogColorW($"Not master");
            yield break;
        }

        foreach (var ((zone, area, type), spawnerGo) in HazardManager.ActiveSpawners)
        {
            if (zone == zoneToLoad && !ReferenceEquals(spawnerGo, null))
            {
                var propSpawner = spawnerGo.GetComponent<PropSpawner>();
                if (ReferenceEquals(propSpawner, null))
                {
                    Plugin.Log.LogColorW($"Could not find prop spawner for {zone.ToString()}/{area.ToString()}/{type.ToString()}");
                    continue;
                }

                int count = 20;
                if (type == HazardType.LavaRiver)
                    count = 10;
                
                var (positions, rotations) = SpawnerLogic.GenerateSpawnPoints(propSpawner, count);
                
                Plugin.Log.LogColor($"Generated {positions.Length} positions out of {count}");
                if (positions.Length == 0)
                {
                    yield return null;
                    continue;
                }
                
                NetComm.Instance.SpawnHazardsNetwork(zone, area, type, positions, rotations);

                yield return new WaitForSeconds(0.33f);
            }
            else
            {
                if (ReferenceEquals(spawnerGo, null))
                {
                    Plugin.Log.LogColorW($"Spawner for {zone.ToString()}/{area.ToString()}/{type.ToString()} is null");
                    yield return null;
                }
            }
        }
    }

}