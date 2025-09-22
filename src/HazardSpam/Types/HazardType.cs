namespace HazardSpam.Types;

public enum HazardType
{
    Unknown,
    
    // Shore
    Urchin,                 // works
    Jelly,                  // works
    
    // Tropics
    PoisonIvy,              // works
    ExploSpore,             // works
    PoisonSpore,            // works
    Thorn,                  // works
    BigThorn,               // works
    Beehive,            // fix
    
    // Alpine
    Geyser,                 // works
    FlashPlant,             // works
    
    // Mesa
    CactusBall,             // works
    Cactus,                 // works
    CactusBig,              // works
    Dynamite,           // fix
    Tumbler,            // fix
    Scorpion,           // fix
    
    // Special
    LavaRiver           // fix
}