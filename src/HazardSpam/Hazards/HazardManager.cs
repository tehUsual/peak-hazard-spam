using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleTools;
using HazardSpam.Config;
using HazardSpam.Helpers;
using HazardSpam.Menu.Settings;
using HazardSpam.Networking;
using HazardSpam.Spawning;
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

    internal static void CreateSpawnersFromConfigOverNetwork()
    {
        List<SpawnIdentifierNet> spawnerNetIds = [];
        
        List<HazardData> allHazards = MenuSettings.GetAllHazards();
        foreach (var hazard in allHazards)
        {
            if (TryCreateSpawnerData(hazard.Zone, hazard.SubZoneArea, hazard.hazardType, out var spawnerDataNet))
            {
                spawnerNetIds.Add(spawnerDataNet);
            }
        }
        
        if (spawnerNetIds.Count > 0)
            NetComm.Instance.CreateMultipleSpawnersNetwork(spawnerNetIds.ToArray());
    }
    
    internal static void CreateSpawner(Zone zone, SubZoneArea area, HazardType hazardType, int viewID)
    {
        // Hack
        if (zone == Zone.Kiln)
        {
            CreateSpawner_Kiln(zone, area, hazardType, viewID);
            return;
        }

        // Normal routine
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

    private static void CreateSpawner_Kiln(Zone zone, SubZoneArea area, HazardType hazardType, int viewID)
    {
        if (!HazardTemplateManager.PropPrefabs.TryGetValue(hazardType, out var propSpawnerPrefab))
        {
            Plugin.Log.LogColorW($"Could not get prefab for '{hazardType.ToString()}'");
            return;
        }

        if (ReferenceEquals(HazardTemplateManager.KilnPropSpawnerLine, null))
        {
            Plugin.Log.LogColorW($"Could not get prop spawner-line for '{hazardType.ToString()}'");
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
        spawnerGo.transform.position = HazardTemplateManager.KilnPropSpawnerLine.transform.position;
        spawnerGo.transform.rotation = HazardTemplateManager.KilnPropSpawnerLine.transform.rotation;
        spawnerGo.transform.localScale = HazardTemplateManager.KilnPropSpawnerLine.transform.localScale;
        
        // Create PhotonView
        if (viewID != -1)
        {
            var pv = spawnerGo.AddComponent<PhotonView>();
            pv.ViewID = viewID;
        }
        
        // Create PropSpawner_Line
        var propSpawnerLine = spawnerGo.AddComponent<PropSpawner_Line>();
        
        // Configure PropSpawner_line
        propSpawnerLine.height = HazardTemplateManager.KilnPropSpawnerLine.height + 150;    // +/- 75
        propSpawnerLine.layerType = HazardTemplateManager.KilnPropSpawnerLine.layerType;
        propSpawnerLine.nrOfSpawns = 1;
        propSpawnerLine.rayCastSpawn = HazardTemplateManager.KilnPropSpawnerLine.rayCastSpawn;
        propSpawnerLine.rayLength = HazardTemplateManager.KilnPropSpawnerLine.rayLength;
        propSpawnerLine.syncTransforms = false;
        propSpawnerLine.props = [propSpawnerPrefab.props[0]];
        
        // Configure this if needed...
        propSpawnerLine.modifiers = new List<PropSpawnerMod>(HazardTemplateManager.KilnPropSpawnerLine.modifiers);

        propSpawnerLine.constraints = new List<PropSpawnerConstraint>()
        {
            new PSC_SameTypeDistance
            {
                minDistance = 5,
                findAllSpawners = false,
                axisMultipliers = Vector3.one
            }
        };

        propSpawnerLine.postConstraints = new List<PropSpawnerConstraintPost>(HazardTemplateManager.KilnPropSpawnerLine.postConstraints);
        
        // Add Trigger Relay
        var triggerRelay = spawnerGo.AddComponent<TriggerRelay>();
        
        // Add to registry
        ActiveSpawners[(zone, area, hazardType)] = spawnerGo;
    }
    
    internal static void ApplyTweaksFromConfigOverNetwork()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        List<HazardTweakNet> tweaks = [];
        
        foreach (var outer in HazardConfigSettings.Configs)
        {
            HazardType type = outer.Key;
            Dictionary<string, object> innerDict = outer.Value;
            foreach (var inner in innerDict)
            {
                string key = inner.Key;
                object value = inner.Value;

                tweaks.Add(new HazardTweakNet(type, key, value));
            }
        }
        
        Plugin.Log.LogColor($"Generated {tweaks.Count} tweaks");

        if (tweaks.Count > 0)
            NetComm.Instance.ApplyTweaksNetwork(tweaks.ToArray());
    }

    internal static void UnloadZone(Zone zoneToUnload)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        Plugin.Log.LogColor($"Unloading zone {zoneToUnload}");
        NetComm.Instance.RemoveSpawnersNetwork(zoneToUnload);
    }

    internal static IEnumerator LoadZone(Zone zoneToLoad)
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;
        
        Plugin.Log.LogColor($"Loading zone {zoneToLoad}");
        
        Dictionary<(SubZoneArea, HazardType), HazardSettings> hazards = MenuSettings.GetHazardsForZone(zoneToLoad);
        foreach (var ((area, type), hazardSettings) in hazards) {
            if (ActiveSpawners.TryGetValue((zoneToLoad, area, type), out var spawnerGo))
            {
                // Volcano hack
                if (zoneToLoad == Zone.Kiln)
                {
                    var propSpawnerLine = spawnerGo.GetComponent<PropSpawner_Line>();
                    if (ReferenceEquals(propSpawnerLine, null))
                    {
                        Plugin.Log.LogColorW($"Could not find prop spawner_line for {zoneToLoad.ToString()}/{area.ToString()}/{type.ToString()}");
                        continue;
                    }
                    
                    var (positions, rotations) = SpawnerLogic.GenerateSpawnPoints(propSpawnerLine, hazardSettings.Amount);
                    Plugin.Log.LogColor($"Generated {positions.Length} positions out of {hazardSettings.Amount}");
                    if (positions.Length == 0)
                    {
                        yield return null;
                        continue;
                    }
                    
                    NetComm.Instance.SpawnHazardsNetwork(zoneToLoad, area, type, positions, rotations);
                    yield return new WaitForSeconds(Plugin.HazardsNetworkSpawnRate);
                }
                
                // Normal routine
                else
                {
                    var propSpawner = spawnerGo.GetComponent<PropSpawner>();
                    if (ReferenceEquals(propSpawner, null))
                    {
                        Plugin.Log.LogColorW($"Could not find prop spawner for {zoneToLoad.ToString()}/{area.ToString()}/{type.ToString()}");
                        continue;
                    }

                    var (positions, rotations) = SpawnerLogic.GenerateSpawnPoints(propSpawner, hazardSettings.Amount);
                    Plugin.Log.LogColor($"Generated {positions.Length} positions out of {hazardSettings.Amount}");
                    if (positions.Length == 0)
                    {
                        yield return null;
                        continue;
                    }
                    
                    NetComm.Instance.SpawnHazardsNetwork(zoneToLoad, area, type, positions, rotations);
                    yield return new WaitForSeconds(Plugin.HazardsNetworkSpawnRate);
                }
            }
            else
            {
                Plugin.Log.LogColorW($"Spawner for {zoneToLoad.ToString()}/{area.ToString()}/{type.ToString()} is null");
                continue;
            }
        }
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