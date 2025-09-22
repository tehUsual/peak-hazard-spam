using NetGameState.Level;

namespace HazardSpam.Types;

public readonly struct SpawnIdentifierNet(Zone zone, SubZoneArea area, HazardType type, int viewID = -1)
{
    public readonly Zone Zone = zone;
    public readonly SubZoneArea Area = area;
    public readonly HazardType Type = type;
    public readonly int ViewID = viewID;
    
    public void Deconstruct(out Zone zone, out SubZoneArea area, out HazardType type, out int viewID)
    {
        zone = Zone;
        area = Area;
        type = Type;
        viewID = ViewID;
    }

    public string GenID()
    {
        return $"{Zone.ToString()}/{Area.ToString()}/{Type.ToString()}";
    }
}