using HazardSpam.Types;
using NetGameState.Level;

namespace HazardSpam.Helpers;

public static class Helper
{
    internal static string GenSpawnID(Zone zone, SubZoneArea subZoneArea, HazardType hazardType)
    {
        return $"{zone}/{subZoneArea}/{hazardType}";
    }
}