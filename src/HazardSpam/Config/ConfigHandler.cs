using System.Collections.Generic;
using BepInEx.Configuration;
using HazardSpam.Helpers;
using HazardSpam.Types;
using NetGameState.Level;

namespace HazardSpam.Config;

public static class ConfigHandler
{
    // Spawn rates
    private static readonly Dictionary<(Zone, SubZoneArea, HazardType), ConfigEntry<int>> SpawnRatesDict = [];


    internal static void Init(ConfigFile config)
    {
        // Shore
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Plateau, HazardType.Urchin, 0);
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Plateau, HazardType.Jelly, 1000);
        
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Wall, HazardType.Urchin, 0);
        BindSpawnRates(config, Zone.Shore, SubZoneArea.Wall, HazardType.Jelly, 2000);
        
        // Tropics
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Plateau, HazardType.Thorn, 50);
        
        
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonIvy, 100);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.ExploSpore, 3250);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonSpore, 750);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.Thorn, 30);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.BigThorn, 30);
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.Beehive, 10);
        
        BindSpawnRates(config, Zone.Tropics, SubZoneArea.Wall, HazardType.LavaRiver, 3);
        
        // Alpine
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.Plateau, HazardType.Geyser, 200);
        
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallLeft, HazardType.Geyser, 150);
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallLeft, HazardType.FlashPlant, 150);

        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallRight, HazardType.Geyser, 150);
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallRight, HazardType.FlashPlant, 150);

        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallLeft, HazardType.LavaRiver, 5);
        BindSpawnRates(config, Zone.Alpine, SubZoneArea.WallRight, HazardType.LavaRiver, 5);
        
        // Mesa
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, HazardType.CactusBall, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, HazardType.Cactus, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Plateau, HazardType.CactusBig, 200);

        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, HazardType.Cactus, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, HazardType.CactusBall, 500);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, HazardType.CactusBig, 50);
        BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, HazardType.Dynamite, 200);
        //BindSpawnRates(config, Zone.Mesa, SubZoneArea.Wall, HazardType.DynamiteOutside, 200);
    }

    private static void BindSpawnRates(ConfigFile config, Zone zone, SubZoneArea subZoneArea, HazardType hazardType, int rate)
    {
        string id = Helper.GenSpawnID(zone, subZoneArea, hazardType);
        var entry = config.Bind("SpawnRates", id, rate, "Attempted spawn count");
        SpawnRatesDict[(zone, subZoneArea, hazardType)] = entry;
    }

    public static int GetSpawnRate(Zone zone, SubZoneArea subZoneArea, HazardType hazardType)
    {
        return SpawnRatesDict.TryGetValue((zone, subZoneArea, hazardType), out var entry) ? entry.Value : 0;
    }
}