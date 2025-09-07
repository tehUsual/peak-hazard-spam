using System.Collections.Generic;
using HazardSpam.Spawning;
using HazardSpam.Types;

namespace HazardSpam.StaticData;

public static class SpawnerDefinitions
{
    public static readonly List<BiomeSpawnDefinition> All = new()
    {
        // Shore
        new (Biome.BiomeType.Shore, "PlateauProps", BiomeArea.Plateau, "Jellies", SpawnType.Jellies),
        new (Biome.BiomeType.Shore, "WallProps", BiomeArea.Wall, "Jellies", SpawnType.Jellies),
        
        // Tropics
        new (Biome.BiomeType.Tropics, "Props_Wall", BiomeArea.Wall, "ExploShrooms", SpawnType.ExploShrooms),
        new (Biome.BiomeType.Tropics, "Props_Wall", BiomeArea.Wall, "PoisonShrooms", SpawnType.PoisonShrooms),
        
        // Alpine
        new (Biome.BiomeType.Alpine, "Rocks/IceRockSpawn_L", BiomeArea.WallLeft, "Geysers", SpawnType.Geysers),
        new (Biome.BiomeType.Alpine, "Rocks/IceRockSpawn_L", BiomeArea.WallLeft, "FlashPlant", SpawnType.FlashPlant),
        
        new (Biome.BiomeType.Alpine, "Rocks/IceRockSpawn_R", BiomeArea.WallRight, "Geysers", SpawnType.Geysers),
        new (Biome.BiomeType.Alpine, "Rocks/IceRockSpawn_R", BiomeArea.WallRight, "FlashPlant", SpawnType.FlashPlant),
        
        new (Biome.BiomeType.Alpine, "", BiomeArea.WallLeft, "LavaRiver", SpawnType.LavaRiver),
        new (Biome.BiomeType.Alpine, "", BiomeArea.WallRight, "LavaRiver (1)", SpawnType.LavaRiver),
        
        //new (Biome.BiomeType.Alpine, "PleateauProps", BiomeArea.Plateau, "Geysers", SpawnType.Geysers),
        
        // Mesa
        new (Biome.BiomeType.Mesa, "Platteau/Props", BiomeArea.Plateau, "Cactus_Balls (1)", SpawnType.CactusBalls),
        new (Biome.BiomeType.Mesa, "Platteau/Props", BiomeArea.Plateau, "Tumblers", SpawnType.Tumblers),
        
        new (Biome.BiomeType.Mesa, "Wall/Props", BiomeArea.Wall, "Cactus_Balls", SpawnType.CactusBalls),
        new (Biome.BiomeType.Mesa, "Wall/Props", BiomeArea.Wall, "Dynamite", SpawnType.Dynamite),
        new (Biome.BiomeType.Mesa, "Wall/Props", BiomeArea.Wall, "Dynamite_Outside", SpawnType.DynamiteOutside),
        new (Biome.BiomeType.Mesa, "Wall/Props", BiomeArea.Wall, "Scorpions", SpawnType.Scorpions),
    };
    
    public static readonly BiomeSpawnDefinition AlpineGeyserPlateau =
        new (Biome.BiomeType.Alpine, "PlateauProps", BiomeArea.Plateau, "Geysers", SpawnType.Geysers);
}