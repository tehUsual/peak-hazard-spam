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
    Beehive,            // needs fixing
    
    // Alpine
    Geyser,                 // works
    FlashPlant,             // works
    
    // Mesa
    CactusBall,         // no hazard, becomes a snowball on pickup
    Cactus,                 // works
    CactusBig,              // works
    Dynamite,           // needs fixing
    Tumbler,            // needs fixing
    Scorpion,           // needs fixing
    
    // Special
    LavaRiver           // needs fixing
}