using System.Collections.Generic;
using BepInEx.Configuration;
using HazardSpam.LevelStructure;
using HazardSpam.Types;
using NetGameState.Level;

namespace HazardSpam.Config;

public static class ConfigHandler
{
    // Spawn rates
    private static readonly Dictionary<(Zone, SubZoneArea, SpawnType), ConfigEntry<int>> SpawnRatesDict = [];


    internal static void Init(ConfigFile config)
    {
        // Shore
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Plateau, SpawnType.Urchin, 0);
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Plateau, SpawnType.Jelly, 1000);
        
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Wall, SpawnType.Urchin, 0);
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Wall, SpawnType.Jelly, 2000);
        
        // Tropics
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Plateau, SpawnType.Thorn, 50);
        
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, SpawnType.PoisonIvy, 100);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, SpawnType.ExploSpore, 3250);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, SpawnType.PoisonSpore, 750);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, SpawnType.Thorn, 30);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, SpawnType.Beehive, 10);
        
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, SpawnType.LavaRiver, 3);
        
        // Alpine
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.Plateau, SpawnType.Geyser, 200);
        
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallLeft, SpawnType.Geyser, 150);
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallLeft, SpawnType.FlashPlant, 150);

        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallRight, SpawnType.Geyser, 150);
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallRight, SpawnType.FlashPlant, 150);

        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallLeft, SpawnType.LavaRiver, 5);
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallRight, SpawnType.LavaRiver, 5);
        
        // Mesa
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, SpawnType.CactusBall, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, SpawnType.Cactus, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, SpawnType.CactusBig, 200);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, SpawnType.CactusBigDry, 100);

        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, SpawnType.Cactus, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, SpawnType.CactusBall, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, SpawnType.CactusBig, 50);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, SpawnType.CactusBigDry, 50);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, SpawnType.Dynamite, 200);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, SpawnType.DynamiteOutside, 200);
    }

    private static void BindSpawnRates(ConfigFile config, Zone zone, SubZoneArea subZoneArea, SpawnType spawnType, int rate)
    {
        string id = SpawnerRegistry.GenSpawnID(zone, subZoneArea, spawnType);
        var entry = config.Bind("SpawnRates", id, rate, "Attempted spawn count");
        SpawnRatesDict[(zone, subZoneArea, spawnType)] = entry;
    }

    public static int GetSpawnRate(Zone zone, SubZoneArea subZoneArea, SpawnType spawnType)
    {
        return SpawnRatesDict.TryGetValue((zone, subZoneArea, spawnType), out var entry) ? entry.Value : 0;
    }
}