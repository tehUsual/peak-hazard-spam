using System.Collections.Generic;
using HazardSpam.Level.Spawner;
using UnityEngine;

namespace HazardSpam.Level.Biomes;

public class BiomeInfo
{
    public Biome.BiomeType BiomeType { get; }
    public string BiomeVariant { get; }

    public Transform Segment { get; }
    public Transform ActiveBiome { get; }

    public List<SpawnerData> Spawners { get; } = [];


    public BiomeInfo(Biome.BiomeType biomeType, string biomeVariant, Transform segment, Transform activeBiome)
    {
        BiomeType = biomeType;
        BiomeVariant = biomeVariant;
        Segment = segment;
        ActiveBiome = activeBiome;
        
        InitSpawners();
    }

    public void Clear()
    {
        foreach (var spawner in Spawners)
        {
            spawner.Clear();
        }
        Spawners.Clear();
    }

    private void InitSpawners()
    {
        Spawners.AddRange(SpawnerFinderAndInit.GetSpawnersFromBiomeType(ActiveBiome, BiomeType, BiomeVariant));
    }
}