using System.Collections.Generic;
using HazardSpam.Spawning;
using HazardSpam.Types;

namespace HazardSpam.StaticData;

public static class SpawnRates
{
    private static readonly Dictionary<(Biome.BiomeType, BiomeArea, SpawnType), int> All = new() 
    {
        // Shore
        [(Biome.BiomeType.Shore, BiomeArea.Plateau, SpawnType.Jellies)] = 1000,
        
        [(Biome.BiomeType.Shore, BiomeArea.Wall, SpawnType.Jellies)] = 2000,
        
        // Tropics
        [(Biome.BiomeType.Tropics, BiomeArea.Wall, SpawnType.ExploShrooms)] = 2000,
        [(Biome.BiomeType.Tropics, BiomeArea.Wall, SpawnType.PoisonShrooms)] = 2000,
        
        // Alpine
        [(Biome.BiomeType.Alpine, BiomeArea.WallLeft, SpawnType.LavaRiver)] = 7,
        [(Biome.BiomeType.Alpine, BiomeArea.WallRight, SpawnType.LavaRiver)] = 7,
        
        [(Biome.BiomeType.Alpine, BiomeArea.WallLeft, SpawnType.Geysers)] = 150,
        [(Biome.BiomeType.Alpine, BiomeArea.WallLeft, SpawnType.FlashPlant)] = 150,
        
        [(Biome.BiomeType.Alpine, BiomeArea.WallRight, SpawnType.Geysers)] = 150,
        [(Biome.BiomeType.Alpine, BiomeArea.WallRight, SpawnType.FlashPlant)] = 150,
        
        [(Biome.BiomeType.Alpine, BiomeArea.Plateau, SpawnType.Geysers)] = 250,
        
        // Mesa
        [(Biome.BiomeType.Mesa, BiomeArea.Plateau, SpawnType.CactusBalls)] = 500,
        [(Biome.BiomeType.Mesa, BiomeArea.Plateau, SpawnType.Tumblers)] = 5,
        
        [(Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.CactusBalls)] = 750,
        [(Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.Dynamite)] = 150,
        [(Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.DynamiteOutside)] = 150,
        [(Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.Scorpions)] = 10,
    };

    public static int GetSpawnRate(Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType)
    {
        return All.GetValueOrDefault((biomeType, area, spawnType), 0);
    }
}
