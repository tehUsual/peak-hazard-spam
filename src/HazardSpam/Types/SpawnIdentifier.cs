using NetGameState.Level;

namespace HazardSpam.Types;

public struct SpawnIdentifier(Zone zone, SubZoneArea area, HazardType type)
{
    public Zone Zone = zone;
    public SubZoneArea Area = area;
    public HazardType Type = type;

    public void Deconstruct(out Zone zone, out SubZoneArea area, out HazardType type)
    {
        zone = Zone;
        area = Area;
        type = Type;
    }

    public string GenID()
    {
        return $"{Zone.ToString()}/{Area.ToString()}/{Type.ToString()}";
    }
}