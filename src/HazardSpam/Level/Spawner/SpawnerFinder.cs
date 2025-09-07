using System.Collections.Generic;
using HazardSpam.Spawning;
using HazardSpam.StaticData;
using HazardSpam.Types;
using UnityEngine;

namespace HazardSpam.Level.Spawner;

public static class SpawnerFinderAndInit
{
    
    public static List<SpawnerData> GetSpawnersFromBiomeType(Transform activeBiome, Biome.BiomeType biomeType, string biomeVariant)
    {
        List<SpawnerData> spawners = [];

        foreach (var biomeDefinition in SpawnerDefinitions.All)
        {
            if (biomeDefinition.BiomeType == biomeType)
                spawners.AddRange(GetSpawnersFromDefinition(activeBiome, biomeDefinition));
        }


        if (biomeType == Biome.BiomeType.Alpine && biomeVariant != "Geysers")
        {
            spawners.AddRange(HandleAlpinePlateauGeyser(activeBiome, SpawnerDefinitions.AlpineGeyserPlateau));
        }
        
        foreach (var spawner in spawners)
            Plugin.Log.LogInfo($"Added spawner '{spawner.SpawnType}' from {biomeType}/{biomeVariant}/{spawner.Area}");
        
        return spawners;
    }

    private static IEnumerable<SpawnerData> HandleAlpinePlateauGeyser(Transform activeBiome, BiomeSpawnDefinition bsd)
    {
        var geyser = activeBiome.parent.Find("GeyserHell/PlateauProps/Geysers");
        if (geyser == null || geyser.GetComponent<PropSpawner>() == null) yield break;
        
        var plateau = activeBiome.Find("PlateauProps");
        if (plateau == null) yield break;
        
        var newGeyser = Object.Instantiate(geyser.gameObject, plateau, worldPositionStays: true);
        newGeyser.SetActive(true);

        SpawnerData? spawnerData = GetSpawnerFromObject(newGeyser, bsd.BiomeType, bsd.Area, bsd.SpawnType);
        if (spawnerData != null) yield return spawnerData;
    }

    private static IEnumerable<SpawnerData> GetSpawnersFromDefinition(Transform activeBiome, BiomeSpawnDefinition bsd)
    {
        return GetSpawnersFromPath(activeBiome, bsd.BiomeType, bsd.Path, bsd.Area, [(bsd.SearchName, bsd.SpawnType)]);
    }

    private static IEnumerable<SpawnerData> GetSpawnersFromPath(Transform activeBiome, Biome.BiomeType biomeType, string path, BiomeArea area, List<(string searcName, SpawnType spawnType)> spawnersToSearch)
    {
        var parent = activeBiome.Find(path);
        if (parent == null)
        {
            Plugin.Log.LogWarning($"Could not find path '{path}' in {activeBiome.name}");
            yield break;
        }

        foreach (Transform child in parent)
        {
            foreach (var (searchName, spawnType) in spawnersToSearch)
            {
                if (child.name == searchName)
                {
                    SpawnerData? spawnerData = GetSpawnerFromObject(child.gameObject, biomeType, area, spawnType);
                    if (spawnerData == null)
                    {
                        Plugin.Log.LogWarning($"Could not find spawner '{spawnType.ToString()}' in {child.name} from {path}, biome {activeBiome.name}");
                        continue;
                    }

                    yield return spawnerData;
                }
            }
        }
    }
    
    public static SpawnerData? GetSpawnerFromObject(GameObject go, Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType)
    {
        var comp = go.GetComponent<PropSpawner>();
        if (comp == null || comp.props.Length == 0) return null;
        
        return new SpawnerData(comp, comp.props[0], biomeType, area, spawnType);
    }
}