using System.Linq;
using UnityEngine;
using HazardSpam.Types;
using HazardSpam.Menu.Descriptors;
using NetGameState.Level;

namespace HazardSpam.Menu.TabContent;

/// <summary>
/// Generic tab content that works with any zone descriptor
/// </summary>
public class GenericTabContent : BaseTabContent
{
    private readonly ZoneDescriptor _descriptor;
    
    public GenericTabContent(RectTransform parent, ZoneDescriptor descriptor) 
        : base(parent, descriptor.DisplayName, descriptor.SubZones)
    {
        _descriptor = descriptor;
    }
    
    protected override void CreateContent()
    {
        // Create horizontal tabs for SubZoneAreas using descriptor
        CreateHorizontalTabs();
    }
    
    /// <summary>
    /// Override to use descriptor's allowed spawn types
    /// </summary>
    protected override string[] GetAllowedSpawnTypeNames(Zone zone, SubZoneArea subZoneArea)
    {
        // Use descriptor's whitelist if available, otherwise fall back to base implementation
        if (_descriptor?.SpawnTypeWhitelist != null && _descriptor.SpawnTypeWhitelist.Length > 0)
        {
            return _descriptor.SpawnTypeWhitelist.Select(s => s.ToString()).ToArray();
        }
        
        return base.GetAllowedSpawnTypeNames(zone, subZoneArea);
    }
}
