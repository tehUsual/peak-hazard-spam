using System.Collections.Generic;
using BepInEx.Configuration;
using HazardSpam.Spawning;
using HazardSpam.Types;

namespace HazardSpam.Config;

public static class ConfigHandler
{
    // Debug
    private static ConfigEntry<bool> _configDebug = null!;
    public static bool Debug => _configDebug.Value;
    
    // SpawnRates
    private static readonly Dictionary<(Biome.BiomeType, BiomeArea, SpawnType), ConfigEntry<int>> SpawnRatesDict = [];
    
    public static void Init(ConfigFile config)
    {
        _configDebug = config.Bind("Debug", "Debug", false, "Enabled debug logging and utils");
        
        InitSpawnRates(config);
    }
    
    private static void InitSpawnRates(ConfigFile config)
    {
        
        // Shore
        BindSpawnRates(config, Biome.BiomeType.Shore, BiomeArea.Plateau, SpawnType.Jellies, 1000);
        BindSpawnRates(config, Biome.BiomeType.Shore, BiomeArea.Wall, SpawnType.Jellies, 2000);

        // Tropics
        BindSpawnRates(config, Biome.BiomeType.Tropics, BiomeArea.Wall, SpawnType.ExploShrooms, 2000);
        BindSpawnRates(config, Biome.BiomeType.Tropics, BiomeArea.Wall, SpawnType.PoisonShrooms, 2000);

        // Alpine
        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.WallLeft, SpawnType.LavaRiver, 7);
        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.WallRight, SpawnType.LavaRiver, 7);

        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.WallLeft, SpawnType.Geysers, 150);
        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.WallLeft, SpawnType.FlashPlant, 150);

        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.WallRight, SpawnType.Geysers, 150);
        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.WallRight, SpawnType.FlashPlant, 150);

        BindSpawnRates(config, Biome.BiomeType.Alpine, BiomeArea.Plateau, SpawnType.Geysers, 250);

        // Mesa
        BindSpawnRates(config, Biome.BiomeType.Mesa, BiomeArea.Plateau, SpawnType.CactusBalls, 500);
        BindSpawnRates(config, Biome.BiomeType.Mesa, BiomeArea.Plateau, SpawnType.Tumblers, 5);

        BindSpawnRates(config, Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.CactusBalls, 750);
        BindSpawnRates(config, Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.Dynamite, 200);
        BindSpawnRates(config, Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.DynamiteOutside, 200);
        BindSpawnRates(config, Biome.BiomeType.Mesa, BiomeArea.Wall, SpawnType.Scorpions, 10);
    }

    private static void BindSpawnRates(ConfigFile config, Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType, int defaultValue)
    {
        string key = $"{biomeType}.{area}-{spawnType}";
        var entry = config.Bind("SpawnRates", key, defaultValue, "Attempted spawn rate, will not guarantee this amount");
        SpawnRatesDict[(biomeType, area, spawnType)] = entry;
    }

    public static int GetSpawnRate(Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType)
    {
        return SpawnRatesDict.TryGetValue((biomeType, area, spawnType), out var entry) ? entry.Value : 0;
    }
}