using System.Collections.Generic;
using BepInEx.Configuration;
using HazardSpam.Helpers;
using HazardSpam.Types;
using NetGameState.Level;

namespace HazardSpam.Config;

public static class ConfigHandler
{
    public static ConfigEntry<bool> Debug = null!;
    public static ConfigEntry<bool> DebugVerbose = null!;
    public static ConfigEntry<bool> DebugMenu = null!;
    public static ConfigEntry<float> HazardsNetworkSpawnRate = null!;

    internal static void Init(ConfigFile config)
    {
        Debug = config.Bind("Debugging","debug", false, "Enable debug logging");
        DebugVerbose = config.Bind("Debugging","verbose", false, "Enable verbose debug logging");
        DebugMenu = config.Bind("Debugging","menu", false, "Enable menu debug logging");
        HazardsNetworkSpawnRate = config.Bind("Hazard Spawning","hazardsNetworkSpawnRate",0.33f, "The delay in seconds between hazard spawn network calls");
    }
}