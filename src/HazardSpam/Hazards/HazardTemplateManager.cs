using System;
using System.Collections.Generic;
using ConsoleTools;
using HazardSpam.Types;
using NetGameState.Level;
using UnityEngine;
using NetGameState.LevelStructure;

namespace HazardSpam.Hazards;

public static class HazardTemplateManager
{
    private static readonly Dictionary<(Zone, SubZoneArea, HazardType), string> Paths = [];

    internal static readonly Dictionary<HazardType, PropSpawner> PropPrefabs = [];
    internal static readonly Dictionary<(Zone, SubZoneArea), PropSpawner> PropSpawners = [];
    internal static PropSpawner_Line? KilnPropSpawnerLine { get; private set; }

    internal static readonly Dictionary<Zone, Transform> BiomeSpawnerRoots = [];

    internal static void Reset()
    {
        PropPrefabs.Clear();
        PropSpawners.Clear();
        BiomeSpawnerRoots.Clear();
        KilnPropSpawnerLine = null;
    }

    internal static void Initialize()
    {
        InitializePaths();
    }

    private static void InitializePaths()       // not used
    {
        // Shore
        Paths[(Zone.Shore, SubZoneArea.Plateau, HazardType.Urchin)] = $"{MapObjectPaths.SegShore}/Default/PlateauProps/Urchins";
        Paths[(Zone.Shore, SubZoneArea.Plateau, HazardType.Jelly)] = $"{MapObjectPaths.SegShore}/Default/PlateauProps/Jellies";
        Paths[(Zone.Shore, SubZoneArea.Wall, HazardType.Urchin)] = $"{MapObjectPaths.SegShore}/Default/WallProps/Urchins";
        Paths[(Zone.Shore, SubZoneArea.Wall, HazardType.Jelly)] = $"{MapObjectPaths.SegShore}/Default/WallProps/Jellies";
        
        // Tropics
        Paths[(Zone.Tropics, SubZoneArea.Plateau, HazardType.Thorn)] = $"{MapObjectPaths.SegTropics}/Thorny/Pops_Plat/Thorns";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonIvy)] = $"{MapObjectPaths.SegTropics}/Default/Props_Wall/Ivy";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.ExploSpore)] = $"{MapObjectPaths.SegTropics}/Default/Props_Wall/ExploShrooms";    // typo
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonSpore)] = $"{MapObjectPaths.SegTropics}/Default/Props_Wall/PoisonShrooms";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.Thorn)] = $"{MapObjectPaths.SegTropics}/Thorny/Props_Wall/Thorns";
        //Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.NiceThorn)] = $"{MapObjectPaths.SegTropics}/Thorny/Props_Wall/NiceThorns";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.BigThorn)] = $"{MapObjectPaths.SegTropics}/Thorny/Props_Wall/BigThorns";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.Beehive)] = $"{MapObjectPaths.SegTropics}/Default/Props_Wall/Behive";     // typo
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.LavaRiver)] = $"{MapObjectPaths.SegTropics}/Lava/LavaRivers";

        // Alpine
        Paths[(Zone.Alpine, SubZoneArea.Plateau, HazardType.Geyser)] = $"{MapObjectPaths.SegAlpine}/GeyserHell/PlateauProps/Geysers";
        Paths[(Zone.Alpine, SubZoneArea.Wall, HazardType.LavaRiver)] = $"{MapObjectPaths.SegAlpine}/Lava/LavaRivers";
        Paths[(Zone.Alpine, SubZoneArea.WallLeft, HazardType.Geyser)] = $"{MapObjectPaths.SegAlpine}/*/Rocks/IceRockSpawn_L/Geysers";
        Paths[(Zone.Alpine, SubZoneArea.WallLeft, HazardType.FlashPlant)] = $"{MapObjectPaths.SegAlpine}/*/Rocks/IceRockSpawn_L/FlashPlant";
        Paths[(Zone.Alpine, SubZoneArea.WallRight, HazardType.Geyser)] = $"{MapObjectPaths.SegAlpine}/*/Rocks/IceRockSpawn_R/Geysers";
        Paths[(Zone.Alpine, SubZoneArea.WallRight, HazardType.FlashPlant)] = $"{MapObjectPaths.SegAlpine}/*/Rocks/IceRockSpawn_R/FlashPlant";
        
        // Mesa
        Paths[(Zone.Mesa, SubZoneArea.Plateau, HazardType.Cactus)] = $"{MapObjectPaths.SegMesa}/Platteau/Props/Cactus"; // typo
        Paths[(Zone.Mesa, SubZoneArea.Plateau, HazardType.CactusBall)] = $"{MapObjectPaths.SegMesa}/Platteau/Props/Cactus_Balls (1)";   // typo
        Paths[(Zone.Mesa, SubZoneArea.Plateau, HazardType.CactusBig)] = $"{MapObjectPaths.SegMesa}/Platteau/Props/Cactus_Big";  // typo

        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.Cactus)] = $"{MapObjectPaths.SegMesa}/Wall/Props/Cactus";
        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.CactusBall)] = $"{MapObjectPaths.SegMesa}/Wall/Props/Cactus_Balls";
        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.CactusBig)] = $"{MapObjectPaths.SegMesa}/Wall/Props/Cactus_Big";
        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.Dynamite)] = $"{MapObjectPaths.SegMesa}/Wall/Props/Dynamite";
        //Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.DynamiteOutside)] = $"{MapObjectPaths.SegMesa}/Wall/Props/Dynamite_Outside";

        // Caldera
        //Paths[(Zone.Caldera, SubZoneArea.Plateau, SpawnType.Egg)] = $"{SegCaldera}/Props/Eggs";
    }

    internal static void LoadSceneData()
    {
        CreateSpawnerRoots();
        LoadPrefabs();
        LoadAreas();
    }

    private static void CreateSpawnerRoots()
    {
        var rootGo = new GameObject("HazardSpamSpawners");
        foreach (Zone zone in Enum.GetValues(typeof(Zone)))
        {
            if ((int)zone < 1)
                continue;

            var zoneGo = new GameObject(zone.ToString());
            zoneGo.transform.SetParent(rootGo.transform);
            BiomeSpawnerRoots.Add(zone, zoneGo.transform);
        }
    }

    private static void LoadPrefabs()
    {
        // Shore
        LoadPropPrefabs(HazardType.Urchin, $"{MapObjectPaths.SegShore}/Default/PlateauProps/Urchins");
        LoadPropPrefabs(HazardType.Jelly, $"{MapObjectPaths.SegShore}/Default/PlateauProps/Jellies");
        // Tropics
        LoadPropPrefabs(HazardType.PoisonIvy, $"{MapObjectPaths.SegTropics}/Default/Props_Wall/Ivy");
        LoadPropPrefabs(HazardType.ExploSpore, $"{MapObjectPaths.SegTropics}/Default/Props_Wall/ExploShrooms"); // typo
        LoadPropPrefabs(HazardType.PoisonSpore, $"{MapObjectPaths.SegTropics}/Default/Props_Wall/PoisonShrooms");
        LoadPropPrefabs(HazardType.Thorn, $"{MapObjectPaths.SegTropics}/Thorny/Props_Wall/Thorns");
        LoadPropPrefabs(HazardType.BigThorn, $"{MapObjectPaths.SegTropics}/Thorny/Props_Wall/BigThorns");
        LoadPropPrefabs(HazardType.Beehive, $"{MapObjectPaths.SegTropics}/Default/Props_Wall/Behive");  // typo
        LoadPropPrefabs(HazardType.LavaRiver, $"{MapObjectPaths.SegTropics}/Lava/LavaRivers");
        // Roots
        /*LoadPropPrefabs(HazardType.ZombieSpawner, $"{MapObjectPaths.SegRoots}/PlateauProps/Zombie Spawners");   // PropSpawner, PhotonView, TriggerRelay
        LoadPropPrefabs(HazardType.Spiders, $"{MapObjectPaths.SegRoots}/WallProps/Spiders");   // PropSpawner, PhotonView, TriggerRelay
        LoadPropPrefabs(HazardType.SporeSpore, $"{MapObjectPaths.SegRoots}/WallProps/Spore Shrooms");   // PropSpawner, PhotonView, TriggerRelay
        LoadPropPrefabs(HazardType.Beetle, $"{MapObjectPaths.SegRoots}/WallProps/Beetles");   // PropSpawner*/
        // Alpine
        LoadPropPrefabs(HazardType.Geyser, $"{MapObjectPaths.SegAlpine}/GeyserHell/PlateauProps/Geysers");
        LoadPropPrefabs(HazardType.FlashPlant, $"{MapObjectPaths.SegAlpine}/Default/Rocks/IceRockSpawn_L/FlashPlant");
        // Mesa
        LoadPropPrefabs(HazardType.Cactus, $"{MapObjectPaths.SegMesa}/Platteau/Props/Cactus");  // typo
        LoadPropPrefabs(HazardType.CactusBall, $"{MapObjectPaths.SegMesa}/Wall/Props/Cactus_Balls");
        LoadPropPrefabs(HazardType.CactusBig, $"{MapObjectPaths.SegMesa}/Wall/Props/Cactus_Big");
        LoadPropPrefabs(HazardType.Dynamite, $"{MapObjectPaths.SegMesa}/Wall/Props/Dynamite");
    }

    private static void LoadAreas()
    {
        // Shore 
        LoadPropSpawnerForArea(Zone.Shore, SubZoneArea.Plateau, $"{MapObjectPaths.SegShore}/Default/PlateauProps/Jellies");
        LoadPropSpawnerForArea(Zone.Shore, SubZoneArea.Wall, $"{MapObjectPaths.SegShore}/Default/WallProps/Jellies");
        // Tropics
        LoadPropSpawnerForArea(Zone.Tropics, SubZoneArea.Plateau, $"{MapObjectPaths.SegTropics}/Default/Pops_Plat/Bushes");
        LoadPropSpawnerForArea(Zone.Tropics, SubZoneArea.Wall, $"{MapObjectPaths.SegTropics}/Default/Props_Wall/ExploShrooms"); // typo
        // Roots
        LoadPropSpawnerForArea(Zone.Roots, SubZoneArea.Plateau, $"{MapObjectPaths.SegRoots}/PlateauProps/ExploShrooms");
        LoadPropSpawnerForArea(Zone.Roots, SubZoneArea.Wall, $"{MapObjectPaths.SegRoots}/WallProps/ExploShrooms (1)");
        // Alpine
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.Plateau, $"{MapObjectPaths.SegAlpine}/GeyserHell/PlateauProps/Geysers");
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.Wall, $"{MapObjectPaths.SegAlpine}/Lava/LavaRivers");
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.WallLeft, $"{MapObjectPaths.SegAlpine}/Default/Rocks/IceRockSpawn_L/Geysers");
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.WallRight, $"{MapObjectPaths.SegAlpine}/Default/Rocks/IceRockSpawn_R/Geysers");
        // Mesa
        LoadPropSpawnerForArea(Zone.Mesa, SubZoneArea.Plateau, $"{MapObjectPaths.SegMesa}/Platteau/Props/Cactus");  // typo
        LoadPropSpawnerForArea(Zone.Mesa, SubZoneArea.Wall, $"{MapObjectPaths.SegMesa}/Wall/Props/Cactus_Balls");
        // Caldera
        LoadPropSpawnerForArea(Zone.Caldera, SubZoneArea.Plateau, $"{MapObjectPaths.SegCaldera}/Props/Eggs");
        
        // Volcano
        // TODO: Add separate PropSpawner_Line support
        LoadKilnPropSpawnerLine(Zone.Kiln, SubZoneArea.Wall, $"{MapObjectPaths.SegKiln}/Props/LuggageSpawnerLine");
    }

    private static void LoadPropSpawnerForArea(Zone zone, SubZoneArea subZoneArea, string path)
    {
        var spawnerGo = GameObject.Find(path);
        if (ReferenceEquals(spawnerGo, null))
        {
            Plugin.Log.LogColorW($"Could not find spawner for '{zone.ToString()}/{subZoneArea.ToString()}' at {path}");
            return;
        }

        var propSpawner = spawnerGo.GetComponent<PropSpawner>();
        if (ReferenceEquals(propSpawner, null))
        {
            Plugin.Log.LogColorW($"Could not find prop spawner for '{zone.ToString()}/{subZoneArea.ToString()}' at {path}");
            return;
        }
        
        PropSpawners[(zone, subZoneArea)] = propSpawner;
        Plugin.Log.LogColorS($"Loaded PropSpawner for '{zone.ToString()}/{subZoneArea.ToString()}'");
    }

    private static void LoadKilnPropSpawnerLine(Zone zone, SubZoneArea subZoneArea, string path)
    {
        var spawnerGo = GameObject.Find(path);
        if (ReferenceEquals(spawnerGo, null))
        {
            Plugin.Log.LogColorW($"Could not find spawner for '{zone.ToString()}/{subZoneArea.ToString()}' at {path}");
            return;
        }
        
        var propSpawnerLine = spawnerGo.GetComponent<PropSpawner_Line>();
        if (ReferenceEquals(propSpawnerLine, null))
        {
            Plugin.Log.LogColorW($"Could not find prop spawner-line for '{zone.ToString()}/{subZoneArea.ToString()}' at {path}");
            return;
        }

        Plugin.Log.LogColorS($"Loaded PropSpawner_Line for '{zone.ToString()}/{subZoneArea.ToString()}'");
        KilnPropSpawnerLine = propSpawnerLine;
    }

    private static void LoadPropPrefabs(HazardType hazardType, string path)
    {
        var spawnerGo = GameObject.Find(path);
        if (ReferenceEquals(spawnerGo, null))
        {
            Plugin.Log.LogColorW($"Could not find spawner for '{hazardType.ToString()}' at {path}");
            return;
        }

        var propSpawner = spawnerGo.GetComponent<PropSpawner>();
        if (ReferenceEquals(propSpawner, null))
        {
            Plugin.Log.LogColorW($"Could not find prop spawner for '{hazardType.ToString()}' at {path}");
            return;
        }
        
        if (propSpawner.props.Length == 0 || ReferenceEquals(propSpawner.props[0], null))
        {
            Plugin.Log.LogColorW($"Could not find prefab for '{hazardType.ToString()}' at {path}");
            return;
            
        }
        
        PropPrefabs[hazardType] = propSpawner;
        Plugin.Log.LogColorS($"Loaded prefab for '{hazardType.ToString()}'");
    }
    
}