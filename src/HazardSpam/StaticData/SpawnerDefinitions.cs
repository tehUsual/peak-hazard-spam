using System.Collections.Generic;
using HazardSpam.Level;
using HazardSpam.Spawning;
using HazardSpam.Types;

namespace HazardSpam.StaticData;

public static class SpawnerDefinitions
{
    public static readonly List<BiomeSpawnDefinition> All = new()
    {
        // Shore
        new (OurBiome.Shore, "PlateauProps", BiomeArea.Plateau, "Jellies", SpawnType.Jellies),
        new (OurBiome.Shore, "WallProps", BiomeArea.Wall, "Jellies", SpawnType.Jellies),

        // Tropics
        new (OurBiome.Tropics, "Props_Wall", BiomeArea.Wall, "ExploShrooms", SpawnType.ExploShrooms),
        new (OurBiome.Tropics, "Props_Wall", BiomeArea.Wall, "PoisonShrooms", SpawnType.PoisonShrooms),

        // Alpine
        new (OurBiome.Alpine, "Rocks/IceRockSpawn_L", BiomeArea.WallLeft, "Geysers", SpawnType.Geysers),
        new (OurBiome.Alpine, "Rocks/IceRockSpawn_L", BiomeArea.WallLeft, "FlashPlant", SpawnType.FlashPlant),

        new (OurBiome.Alpine, "Rocks/IceRockSpawn_R", BiomeArea.WallRight, "Geysers", SpawnType.Geysers),
        new (OurBiome.Alpine, "Rocks/IceRockSpawn_R", BiomeArea.WallRight, "FlashPlant", SpawnType.FlashPlant),

        new (OurBiome.Alpine, "", BiomeArea.WallLeft, "LavaRiver", SpawnType.LavaRiver),
        new (OurBiome.Alpine, "", BiomeArea.WallRight, "LavaRiver (1)", SpawnType.LavaRiver),

        //new (OurBiome.Alpine, "PleateauProps", BiomeArea.Plateau, "Geysers", SpawnType.Geysers),

        // Mesa
        new (OurBiome.Mesa, "Platteau/Props", BiomeArea.Plateau, "Cactus_Balls (1)", SpawnType.CactusBalls),
        new (OurBiome.Mesa, "Platteau/Props", BiomeArea.Plateau, "Tumblers", SpawnType.Tumblers),

        new (OurBiome.Mesa, "Wall/Props", BiomeArea.Wall, "Cactus_Balls", SpawnType.CactusBalls),
        new (OurBiome.Mesa, "Wall/Props", BiomeArea.Wall, "Dynamite", SpawnType.Dynamite),
        new (OurBiome.Mesa, "Wall/Props", BiomeArea.Wall, "Dynamite_Outside", SpawnType.DynamiteOutside),
        new (OurBiome.Mesa, "Wall/Props", BiomeArea.Wall, "Scorpions", SpawnType.Scorpions),
    };

    public static readonly BiomeSpawnDefinition AlpineGeyserPlateau =
        new (OurBiome.Alpine, "PlateauProps", BiomeArea.Plateau, "Geysers", SpawnType.Geysers);
}