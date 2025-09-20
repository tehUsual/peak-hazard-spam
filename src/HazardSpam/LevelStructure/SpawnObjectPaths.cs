using NetGameState.LevelStructure;

namespace HazardSpam.LevelStructure;

public static class SpawnObjectPaths
{
    public readonly struct Shore
    {
        // Plateau
        public const string PlateauUrchins = "*/PlateauProps/Urchins";
        public const string PlateauJellies = "*/PlateauProps/Jellies";
        
        // Wall
        public const string WallUrchins = "*/WallProps/Urchins";
        public const string WallJellies = "*/WallProps/Jellies";
    }

    public readonly struct Tropics
    {
        // Plateau
        public const string PlateauThorns = "*/Pops_Plat/Thorns";
        
        // Wall
        public const string WallPoisonIvy = "*/Props_Wall/Ivy";
        public const string WallExploSpores = "*/Props_Wall/ExploShrooms";
        public const string WallPoisonSpores = "*/Props_Wall/PoisonShrooms";
        public const string WallThorns = "*/Props_Wall/Thorns";
        public const string WallBeehives = "*/Props_Wall/Behive";

        // (Lava) Wall
        public const string Lava_LavaRivers = "*/LavaRivers";
    }

    public readonly struct Alpine
    {
        // (GeyserHell) Plateau
        public const string GeyserHell_PlateauGeysers = "*/PlateauProps/Geysers";
        
        // Wall Left
        public const string WallLeftGeysers = "*/Rocks/IceRockSpawn_L/Geysers";
        public const string WallLeftFlashPlant = "*/Rocks/IceRockSpawn_L/FlashPlant";
        
        // Wall Right
        public const string WallRightGeysers = "*/Rocks/IceRockSpawn_R/Geysers";
        public const string WallRightFlashPlant = "*/Rocks/IceRockSpawn_R/FlashPlant";
        
        // (Lava) Wall
        public const string Lava_LavaRivers_1 = "*/LavaRivers";
        public const string Lava_LavaRivers_2 = "*/LavaRivers (1)";
    }

    public readonly struct Mesa
    {
        // Plateau
        public const string PlateauCactusBalls = "Platteau/Props/Cactus_Balls (1)";
        public const string PlateauCactus = "Platteau/Props/Cactus";
        public const string PlateauCactusBig = "Platteau/Props/Cactus_Big";
        public const string PlateauCactusBigDry = "Platteau/Props/Cactus_Big_Dry";
        
        // Wall
        public const string WallCactus = "Wall/Props/Cactus";
        public const string WallCactusBalls = "Wall/Props/Cactus_Balls";
        public const string WallCactusBig = "Wall/Props/Cactus_Big";
        public const string WallCactusBigDry = "Wall/Props/Cactus_Big_Dry";
        public const string WallDynamite = "Wall/Props/Dynamite";
        public const string WallDynamiteOutside = "Wall/Props/Dynamite_Outside";
    }


}