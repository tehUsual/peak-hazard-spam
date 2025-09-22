using System;
using System.Collections.Generic;
using ConsoleTools;
using HazardSpam.Types;
using NetGameState.Level;
using UnityEngine;

namespace HazardSpam.Hazards;

public static class HazardTemplateManager
{
    internal static readonly Dictionary<(Zone, SubZoneArea, HazardType), string> Paths = [];

    internal static readonly Dictionary<HazardType, PropSpawner> PropPrefabs = [];
    internal static readonly Dictionary<(Zone, SubZoneArea), PropSpawner> PropSpawners = [];

    internal static readonly Dictionary<Zone, Transform> BiomeSpawnerRoots = [];

    private const string SegBeach = "Map/Biome_1/Beach/Beach_Segment";
    private const string SegTropics = "Map/Biome_2/Jungle/Jungle_Segment";
    private const string SegAlpine = "Map/Biome_3/Snow/Snow_Segment";
    private const string SegMesa = "Map/Biome_3/Desert/Desert_Segment";
    private const string SegCaldera = "Map/Biome_4/Volcano/Caldera_Segment";
    private const string SegKiln = "Map/Biome_4/Volcano/Volcano_Segment";
    

    internal static void Reset()
    {
        PropPrefabs.Clear();
        PropSpawners.Clear();
        BiomeSpawnerRoots.Clear();
    }

    internal static void Initialize()
    {
        InitializePaths();
    }

    private static void InitializePaths()
    {
        // Shore
        Paths[(Zone.Shore, SubZoneArea.Plateau, HazardType.Urchin)] = $"{SegBeach}/Default/PlateauProps/Urchins";
        Paths[(Zone.Shore, SubZoneArea.Plateau, HazardType.Jelly)] = $"{SegBeach}/Default/PlateauProps/Jellies";
        Paths[(Zone.Shore, SubZoneArea.Wall, HazardType.Urchin)] = $"{SegBeach}/Default/WallProps/Urchins";
        Paths[(Zone.Shore, SubZoneArea.Wall, HazardType.Jelly)] = $"{SegBeach}/Default/WallProps/Jellies";
        
        // Tropics
        Paths[(Zone.Tropics, SubZoneArea.Plateau, HazardType.Thorn)] = $"{SegTropics}/Thorny/Pops_Plat/Thorns";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonIvy)] = $"{SegTropics}/Default/Props_Wall/Ivy";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.ExploSpore)] = $"{SegTropics}/Default/Props_Wall/ExploShrooms";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonSpore)] = $"{SegTropics}/Default/Props_Wall/PoisonShrooms";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.Thorn)] = $"{SegTropics}/Thorny/Props_Wall/Thorns";
        //Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.NiceThorn)] = $"{SegTropics}/Thorny/Props_Wall/NiceThorns";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.BigThorn)] = $"{SegTropics}/Thorny/Props_Wall/BigThorns";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.Beehive)] = $"{SegTropics}/Default/Props_Wall/Behive";
        Paths[(Zone.Tropics, SubZoneArea.Wall, HazardType.LavaRiver)] = $"{SegTropics}/Lava/LavaRivers";

        // Alpine
        Paths[(Zone.Alpine, SubZoneArea.Plateau, HazardType.Geyser)] = $"{SegAlpine}/GeyserHell/PlateauProps/Geysers";
        Paths[(Zone.Alpine, SubZoneArea.Wall, HazardType.LavaRiver)] = $"{SegAlpine}/Lava/LavaRivers";
        Paths[(Zone.Alpine, SubZoneArea.WallLeft, HazardType.Geyser)] = $"{SegAlpine}/*/Rocks/IceRockSpawn_L/Geysers";
        Paths[(Zone.Alpine, SubZoneArea.WallLeft, HazardType.FlashPlant)] = $"{SegAlpine}/*/Rocks/IceRockSpawn_L/FlashPlant";
        Paths[(Zone.Alpine, SubZoneArea.WallRight, HazardType.Geyser)] = $"{SegAlpine}/*/Rocks/IceRockSpawn_R/Geysers";
        Paths[(Zone.Alpine, SubZoneArea.WallRight, HazardType.FlashPlant)] = $"{SegAlpine}/*/Rocks/IceRockSpawn_R/FlashPlant";
        
        // Mesa
        Paths[(Zone.Mesa, SubZoneArea.Plateau, HazardType.Cactus)] = $"{SegMesa}/Platteau/Props/Cactus";
        Paths[(Zone.Mesa, SubZoneArea.Plateau, HazardType.CactusBall)] = $"{SegMesa}/Platteau/Props/Cactus_Balls (1)";
        Paths[(Zone.Mesa, SubZoneArea.Plateau, HazardType.CactusBig)] = $"{SegMesa}/Platteau/Props/Cactus_Big";

        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.Cactus)] = $"{SegMesa}/Wall/Props/Cactus";
        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.CactusBall)] = $"{SegMesa}/Wall/Props/Cactus_Balls";
        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.CactusBig)] = $"{SegMesa}/Wall/Props/Cactus_Big";
        Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.Dynamite)] = $"{SegMesa}/Wall/Props/Dynamite";
        //Paths[(Zone.Mesa, SubZoneArea.Wall, HazardType.DynamiteOutside)] = $"{SegMesa}/Wall/Props/Dynamite_Outside";

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
        LoadPropPrefabs(HazardType.Urchin, $"{SegBeach}/Default/PlateauProps/Urchins");
        LoadPropPrefabs(HazardType.Jelly, $"{SegBeach}/Default/PlateauProps/Jellies");
        // Tropics
        LoadPropPrefabs(HazardType.PoisonIvy, $"{SegTropics}/Default/Props_Wall/Ivy");
        LoadPropPrefabs(HazardType.ExploSpore, $"{SegTropics}/Default/Props_Wall/ExploShrooms");
        LoadPropPrefabs(HazardType.PoisonSpore, $"{SegTropics}/Default/Props_Wall/PoisonShrooms");
        LoadPropPrefabs(HazardType.Thorn, $"{SegTropics}/Thorny/Props_Wall/Thorns");
        LoadPropPrefabs(HazardType.BigThorn, $"{SegTropics}/Thorny/Props_Wall/BigThorns");
        LoadPropPrefabs(HazardType.Beehive, $"{SegTropics}/Default/Props_Wall/Behive");
        LoadPropPrefabs(HazardType.LavaRiver, $"{SegTropics}/Lava/LavaRivers");
        // Alpine
        LoadPropPrefabs(HazardType.Geyser, $"{SegAlpine}/GeyserHell/PlateauProps/Geysers");
        LoadPropPrefabs(HazardType.FlashPlant, $"{SegAlpine}/Default/Rocks/IceRockSpawn_L/FlashPlant");
        // Mesa
        LoadPropPrefabs(HazardType.Cactus, $"{SegMesa}/Platteau/Props/Cactus");
        LoadPropPrefabs(HazardType.CactusBall, $"{SegMesa}/Wall/Props/Cactus_Balls");
        LoadPropPrefabs(HazardType.CactusBig, $"{SegMesa}/Wall/Props/Cactus_Big");
        LoadPropPrefabs(HazardType.Dynamite, $"{SegMesa}/Wall/Props/Dynamite");
    }

    private static void LoadAreas()
    {
        // Shore 
        LoadPropSpawnerForArea(Zone.Shore, SubZoneArea.Plateau, $"{SegBeach}/Default/PlateauProps/Jellies");
        LoadPropSpawnerForArea(Zone.Shore, SubZoneArea.Wall, $"{SegBeach}/Default/WallProps/Jellies");
        // Tropics
        LoadPropSpawnerForArea(Zone.Tropics, SubZoneArea.Plateau, $"{SegTropics}/Default/Pops_Plat/Bushes");
        LoadPropSpawnerForArea(Zone.Tropics, SubZoneArea.Wall, $"{SegTropics}/Default/Props_Wall/ExploShrooms");
        // Alpine
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.Plateau, $"{SegAlpine}/GeyserHell/PlateauProps/Geysers");
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.Wall, $"{SegAlpine}/Lava/LavaRivers");
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.WallLeft, $"{SegAlpine}/Default/Rocks/IceRockSpawn_L/Geysers");
        LoadPropSpawnerForArea(Zone.Alpine, SubZoneArea.WallRight, $"{SegAlpine}/Default/Rocks/IceRockSpawn_R/Geysers");
        // Mesa
        LoadPropSpawnerForArea(Zone.Mesa, SubZoneArea.Plateau, $"{SegMesa}/Platteau/Props/Cactus");
        LoadPropSpawnerForArea(Zone.Mesa, SubZoneArea.Wall, $"{SegMesa}/Wall/Props/Cactus_Balls");
        // Caldera
        LoadPropSpawnerForArea(Zone.Caldera, SubZoneArea.Plateau, $"{SegCaldera}/Props/Eggs");
        // Volcano
        LoadPropSpawnerForArea(Zone.Kiln, SubZoneArea.Wall, $"{SegKiln}/Props/LuggageSpawnerLine");     // PropSpawner_Line
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