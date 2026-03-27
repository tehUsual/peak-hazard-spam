using HazardSpam.Types;
using NetGameState.Level;

namespace HazardSpam.Menu.Descriptors;

/// <summary>
/// Describes a zone and its configuration for UI generation
/// </summary>
public class ZoneDescriptor
{
    public Zone Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public SubZoneArea[] SubZones { get; set; } = System.Array.Empty<SubZoneArea>();
    public HazardType[]? SpawnTypeWhitelist { get; set; }
    
    public ZoneDescriptor(Zone id, string displayName, SubZoneArea[] subZones, HazardType[]? spawnTypeWhitelist = null)
    {
        Id = id;
        DisplayName = displayName;
        SubZones = subZones;
        SpawnTypeWhitelist = spawnTypeWhitelist;
    }
}

/// <summary>
/// Describes a sub-zone area within a zone
/// </summary>
public class SubZoneDescriptor
{
    public SubZoneArea Area { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public HazardType[]? AllowedSpawnTypes { get; set; }
    
    public SubZoneDescriptor(SubZoneArea area, string displayName, HazardType[]? allowedSpawnTypes = null)
    {
        Area = area;
        DisplayName = displayName;
        AllowedSpawnTypes = allowedSpawnTypes;
    }
}
