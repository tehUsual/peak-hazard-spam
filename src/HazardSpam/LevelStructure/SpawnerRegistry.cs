using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleTools;
using HazardSpam.Config;
using HazardSpam.Networking;
using HazardSpam.Types;
using NetGameState.Level;
using NetGameState.LevelStructure;
using UnityEngine;

namespace HazardSpam.LevelStructure;

public static class SpawnerRegistry
{
    internal static Dictionary<string, SpawnerType> Spawners { get; } = [];


    internal static void CleanupSpawners()
    {
        Spawners.Clear();
    }

    internal static IEnumerator InitializeSpawnersRoutine(TropicsZone tropicsZone, AlpineZone alpineZone, bool isAlpine)
    {
        // Initialize spawners
        yield return InitializeSpawnerDefinitions(isAlpine);
        int registered = Spawners.Count;
        yield return CopySpecialSpawners(tropicsZone, alpineZone, isAlpine);
        
        Plugin.Log.LogColor($"Registered {registered} spawners. Copied {Spawners.Count - registered} spawners");
    }

    internal static IEnumerator LoadSpawnersRoutine(Dictionary<Zone, SubZone> subZones)
    {
        foreach (var kvp in Spawners)
        {
            kvp.Value.LoadSpawner(subZones[kvp.Value.Zone]);
            yield return null;

            if (kvp.Value.IsValid)
                Plugin.Log.LogColorD($"Loaded spawner '{kvp.Value.ID}'");
            else
                Plugin.Log.LogColorW($"Failed to load spawner '{kvp.Value.ID}'");
        }
    }
    

    internal static IEnumerator LoadZoneRoutine(Zone zone)
    {
        foreach (var kvp in Spawners)
        {
            if (!kvp.Value.IsValid || kvp.Value.Zone != zone)
                continue;

            var (positions, rotations) =
                SpawnerLogic.GenerateSpawnPoints(kvp.Value.PropSpawner!, kvp.Value.AmountToSpawn);
            SpawnerNetwork.Instance.SpawnPropsNetworkNew(kvp.Value.ID, positions, rotations);

            yield return new WaitForSeconds(0.5f);
        }
    }

    internal static IEnumerator UnloadZoneRoutine(Zone zone)
    {
        foreach (var kvp in Spawners)
        {
            if (!kvp.Value.IsValid || !kvp.Value.IsActive || kvp.Value.Zone != zone)
            {
                Plugin.Log.LogColor($"Skipping unload of '{kvp.Value.ID}', Valid: {kvp.Value.IsValid}, Active: {kvp.Value.IsActive}, Zone: {kvp.Value.Zone}, Expected: {zone}");
                continue;
            }

            SpawnerNetwork.Instance.ClearPropsNetworkNew(kvp.Value.ID);

            yield return new WaitForSeconds(0.1f);
        }
    }

    internal static void InstantiateMultipleById(string id, Vector3[] positions, Quaternion[] rotations)
    {
        if (Spawners.TryGetValue(id, out var spawner))
        {
            SpawnerNetwork.Instance.StartCoroutine(spawner.InstantiateChildren(positions, rotations));
        }
    }

    internal static void DestroySpawnsById(string id)
    {
        if (Spawners.TryGetValue(id, out var spawner))
        {
            spawner.DeleteSpawns();
        }
    }

    private static IEnumerator InitializeSpawnerDefinitions(bool isAlpine)
    {
        // === Shore
        RegisterSpawner(Zone.Shore, SubZoneArea.Plateau, SpawnType.Urchin, $"{MapObjectPaths.SegShore}/{SpawnObjectPaths.Shore.PlateauUrchins}");
        RegisterSpawner(Zone.Shore, SubZoneArea.Plateau, SpawnType.Jelly, $"{MapObjectPaths.SegShore}/{SpawnObjectPaths.Shore.PlateauJellies}");
    
        RegisterSpawner(Zone.Shore, SubZoneArea.Wall, SpawnType.Urchin, $"{MapObjectPaths.SegShore}/{SpawnObjectPaths.Shore.WallUrchins}");
        RegisterSpawner(Zone.Shore, SubZoneArea.Wall, SpawnType.Jelly, $"{MapObjectPaths.SegShore}/{SpawnObjectPaths.Shore.WallJellies}");
        yield return null;
        
        
        // === Tropics
        RegisterSpawner(Zone.Tropics, SubZoneArea.Plateau, SpawnType.Thorn, $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.PlateauThorns}");
        
        RegisterSpawner(Zone.Tropics, SubZoneArea.Wall, SpawnType.PoisonIvy, $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.WallPoisonIvy}");
        RegisterSpawner(Zone.Tropics, SubZoneArea.Wall, SpawnType.ExploSpore, $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.WallExploSpores}");
        RegisterSpawner(Zone.Tropics, SubZoneArea.Wall, SpawnType.PoisonSpore, $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.WallPoisonSpores}");
        RegisterSpawner(Zone.Tropics, SubZoneArea.Wall, SpawnType.Thorn, $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.WallThorns}");
        RegisterSpawner(Zone.Tropics, SubZoneArea.Wall, SpawnType.Beehive, $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.WallBeehives}");
        yield return null;
        
        
        if (isAlpine)
        {
            // === Alpine
            RegisterSpawner(Zone.Alpine, SubZoneArea.WallLeft, SpawnType.Geyser, $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.WallLeftGeysers}");
            RegisterSpawner(Zone.Alpine, SubZoneArea.WallLeft, SpawnType.FlashPlant, $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.WallLeftFlashPlant}");
        
            RegisterSpawner(Zone.Alpine, SubZoneArea.WallRight, SpawnType.Geyser, $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.WallRightGeysers}");
            RegisterSpawner(Zone.Alpine, SubZoneArea.WallRight, SpawnType.FlashPlant, $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.WallRightFlashPlant}");
            yield return null;
        }
        else
        {
            // === Mesa
            RegisterSpawner(Zone.Mesa, SubZoneArea.Plateau, SpawnType.CactusBall, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.PlateauCactusBalls}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Plateau, SpawnType.Cactus, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.PlateauCactus}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Plateau, SpawnType.CactusBig, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.PlateauCactusBig}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Plateau, SpawnType.CactusBigDry, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.PlateauCactusBigDry}");
            yield return null;

            RegisterSpawner(Zone.Mesa, SubZoneArea.Wall, SpawnType.Cactus, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.WallCactus}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Wall, SpawnType.CactusBall, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.WallCactusBalls}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Wall, SpawnType.CactusBig, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.WallCactusBig}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Wall, SpawnType.CactusBigDry, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.WallCactusBigDry}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Wall, SpawnType.Dynamite, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.WallDynamite}");
            RegisterSpawner(Zone.Mesa, SubZoneArea.Wall, SpawnType.DynamiteOutside, $"{MapObjectPaths.SegMesa}/{SpawnObjectPaths.Mesa.WallDynamiteOutside}");
            yield return null;
        }
    }

    private static IEnumerator CopySpecialSpawners(TropicsZone tropicsZone, AlpineZone alpineZone, bool isAlpine)
    {
        string fromPath;
        string toPath;
        string instancedPath;

        // Copy tropics
        if (tropicsZone != TropicsZone.Lava)
        {
            fromPath = $"{MapObjectPaths.SegTropics}/{SpawnObjectPaths.Tropics.Lava_LavaRivers}".Replace("*", "Lava");
            toPath = $"{MapObjectPaths.SegTropics}/*".Replace("*", tropicsZone.ToString());
            instancedPath = CopySpawner(fromPath, toPath);
            if (instancedPath.Length > 0)
                RegisterSpawner(Zone.Tropics, SubZoneArea.Wall, SpawnType.LavaRiver, instancedPath);
            else
                Plugin.Log.LogColorW($"Failed to copy '{GenSpawnID(Zone.Tropics, SubZoneArea.Wall, SpawnType.LavaRiver)}' from 'Lava' to '{tropicsZone.ToString()}'");
            instancedPath = "";
            yield return null;
        }


        // Copy alpine geyser
        if (isAlpine && alpineZone != AlpineZone.GeyserHell)
        {
            fromPath = $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.GeyserHell_PlateauGeysers}".Replace("*", "GeyserHell");
            toPath = $"{MapObjectPaths.SegAlpine}/*/PlateauProps".Replace("*", alpineZone.ToString());
            instancedPath = CopySpawner(fromPath, toPath);
            if (instancedPath.Length > 0)
                RegisterSpawner(Zone.Alpine, SubZoneArea.Plateau, SpawnType.Geyser, instancedPath);
            else
                Plugin.Log.LogColorW($"Failed to copy '{GenSpawnID(Zone.Alpine, SubZoneArea.Plateau, SpawnType.Geyser)}' from 'GeyserHell' to '{alpineZone.ToString()}'");
            instancedPath = "";
            yield return null;
        }

        // Copy alpine lava
        if (isAlpine && alpineZone != AlpineZone.Lava)
        {
            fromPath = $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.Lava_LavaRivers_1}".Replace("*", "Lava");
            toPath = $"{MapObjectPaths.SegAlpine}/*".Replace("*", alpineZone.ToString());
            instancedPath = CopySpawner(fromPath, toPath);
            if (instancedPath.Length > 0)
                RegisterSpawner(Zone.Alpine, SubZoneArea.WallLeft, SpawnType.LavaRiver, instancedPath);
            else
                Plugin.Log.LogColorW($"Failed to copy '{GenSpawnID(Zone.Alpine, SubZoneArea.WallLeft, SpawnType.LavaRiver)}/1' from 'Lava' to '{alpineZone.ToString()}'");
            instancedPath = "";
            yield return null;
            
            fromPath = $"{MapObjectPaths.SegAlpine}/{SpawnObjectPaths.Alpine.Lava_LavaRivers_2}".Replace("*", "Lava");
            toPath = $"{MapObjectPaths.SegAlpine}/*".Replace("*", alpineZone.ToString());
            instancedPath = CopySpawner(fromPath, toPath);
            if (instancedPath.Length > 0)
                RegisterSpawner(Zone.Alpine, SubZoneArea.WallRight, SpawnType.LavaRiver, instancedPath);
            else
                Plugin.Log.LogColorW($"Failed to copy '{GenSpawnID(Zone.Alpine, SubZoneArea.WallRight, SpawnType.LavaRiver)}/2' from 'Lava' to '{alpineZone.ToString()}'");
            instancedPath = "";
            yield return null;
        }
    }

    private static string CopySpawner(string fromPath, string toPath)
    {
        var spawnerObj = GameObject.Find(fromPath);
        if (ReferenceEquals(spawnerObj?.GetComponent<PropSpawner>(), null))
        {
            Plugin.Log.LogColorW($"Original spawner '{fromPath}' is null");
            return "";
        }

        var targetObj = GameObject.Find(toPath);
        if (ReferenceEquals(targetObj, null))
        {
            Plugin.Log.LogColorW($"Target spawner '{toPath}' is null");
            return "";    
        }
        
        //CopySpawnerWithoutChildren(spawnerObj, targetObj.transform);
        var newSpawnerObj = Object.Instantiate(spawnerObj, targetObj.transform, worldPositionStays: true);
        
        // delete children
        foreach (Transform child in new List<Transform>(newSpawnerObj.transform.Cast<Transform>()))
        {
            Object.Destroy(child.gameObject);
        }

        newSpawnerObj.SetActive(true);
        return $"{toPath}/{newSpawnerObj.name}";
    }

    private static void RegisterSpawner(Zone zone, SubZoneArea subZoneArea, SpawnType spawnType, string path)
    {
        string id = GenSpawnID(zone, subZoneArea, spawnType);
        int amountToSpawn = ConfigHandler.GetSpawnRate(zone, subZoneArea, spawnType);
        Spawners[id] = new SpawnerType(id, zone, subZoneArea, spawnType, path, amountToSpawn);
    }

    internal static string GenSpawnID(Zone zone, SubZoneArea subZoneArea, SpawnType spawnType)
    {
        return $"{zone}/{subZoneArea}/{spawnType}";
    }
}