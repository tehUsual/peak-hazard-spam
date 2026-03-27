using System.Collections.Generic;
using System.Linq;
using HazardSpam.Types;
using NetGameState.Level;

namespace HazardSpam.Menu.Descriptors;

/// <summary>
/// Static registry of zone descriptors for UI generation
/// </summary>
public static class ZoneDescriptors
{
    private static readonly List<ZoneDescriptor> _descriptors = new()
    {
        new ZoneDescriptor(
            Zone.Shore,
            "Shore",
            new[] { SubZoneArea.Plateau, SubZoneArea.Wall },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),
        new ZoneDescriptor(
            Zone.Tropics,
            "Tropics",
            new[] { SubZoneArea.Plateau, SubZoneArea.Wall },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),
        // Not yet implemented:
        /*new ZoneDescriptor(
            Zone.Roots,
            "Roots",
            new[] { SubZoneArea.Plateau, SubZoneArea.Wall },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),*/
        new ZoneDescriptor(
            Zone.Alpine,
            "Alpine",
            new[] { SubZoneArea.Plateau, SubZoneArea.Wall, SubZoneArea.WallLeft, SubZoneArea.WallRight },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),
        new ZoneDescriptor(
            Zone.Mesa,
            "Mesa",
            new[] { SubZoneArea.Plateau, SubZoneArea.Wall },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),
        new ZoneDescriptor(
            Zone.Caldera,
            "Caldera",
            new[] { SubZoneArea.Plateau },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),
        // Not yet implemented:
        /*new ZoneDescriptor(
            Zone.Kiln,
            "Kiln",
            new[] { SubZoneArea.Wall },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        ),*/
        // Not yet implemented:
        /*new ZoneDescriptor(
            Zone.Peak,
            "Peak",
            new[] { SubZoneArea.Plateau },
            spawnTypeWhitelist: new[] { HazardType.Urchin, HazardType.Jelly, HazardType.PoisonIvy,
                HazardType.ExploSpore, HazardType.PoisonSpore, HazardType.Thorn, HazardType.BigThorn, HazardType.Geyser,
                HazardType.FlashPlant, HazardType.CactusBall, HazardType.Cactus, HazardType.CactusBig }
        )*/
    };
    
    /// <summary>
    /// Get all zone descriptors
    /// </summary>
    public static IEnumerable<ZoneDescriptor> GetAll()
    {
        return _descriptors.AsReadOnly();
    }
    
    /// <summary>
    /// Get a zone descriptor by zone ID
    /// </summary>
    public static ZoneDescriptor? GetByZone(Zone zone)
    {
        return _descriptors.FirstOrDefault(d => d.Id == zone);
    }
    
    /// <summary>
    /// Get all zones that should be displayed in the UI (excluding Unknown and Any)
    /// </summary>
    public static IEnumerable<ZoneDescriptor> GetDisplayableZones()
    {
        return _descriptors.Where(d => d.Id != Zone.Unknown && d.Id != Zone.Any);
    }
    
    /// <summary>
    /// Get allowed spawn types for a zone, respecting whitelist if present
    /// </summary>
    public static HazardType[] GetAllowedSpawnTypes(Zone zone)
    {
        var descriptor = GetByZone(zone);
        if (descriptor?.SpawnTypeWhitelist != null)
        {
            return descriptor.SpawnTypeWhitelist;
        }
        
        // Return empty array if no descriptor found
        return new HazardType[0];
    }
    
    /// <summary>
    /// Get allowed spawn type names for a zone
    /// </summary>
    public static string[] GetAllowedSpawnTypeNames(Zone zone)
    {
        return GetAllowedSpawnTypes(zone).Select(s => s.ToString()).ToArray();
    }
    
    /// <summary>
    /// Get sub-zones for a zone
    /// </summary>
    public static SubZoneArea[] GetSubZones(Zone zone)
    {
        var descriptor = GetByZone(zone);
        if (descriptor?.SubZones != null)
        {
            return descriptor.SubZones;
        }
        
        // Return empty array if no descriptor found
        return new SubZoneArea[0];
    }
    
    /// <summary>
    /// Convert string to Zone enum (case-insensitive)
    /// </summary>
    public static Zone? ParseZone(string zoneName)
    {
        if (string.IsNullOrEmpty(zoneName)) return null;
        
        if (System.Enum.TryParse<Zone>(zoneName, true, out var zone))
        {
            return zone;
        }
        
        return null;
    }
    
    /// <summary>
    /// Get all displayable zone enums (excludes Unknown and Any)
    /// </summary>
    public static Zone[] GetDisplayableZoneEnums()
    {
        return System.Enum.GetValues(typeof(Zone))
            .Cast<Zone>()
            .Where(z => z != Zone.Unknown && z != Zone.Any)
            .ToArray();
    }
    
    /// <summary>
    /// Get all displayable zone names
    /// </summary>
    public static string[] GetDisplayableZoneNames()
    {
        return GetDisplayableZoneEnums().Select(z => z.ToString()).ToArray();
    }
    
    /// <summary>
    /// Get display name for a zone
    /// </summary>
    public static string GetDisplayName(Zone zone)
    {
        return zone.ToString();
    }
}
