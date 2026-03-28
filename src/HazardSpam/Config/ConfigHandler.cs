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

    internal static void Init(ConfigFile config)
    {
        Debug = config.Bind("Debugging","debug", true, "Enable debug logging");
        DebugVerbose = config.Bind("Debugging","verbose", true, "Enable verbose debug logging");
        DebugMenu = config.Bind("Debugging","menu", false, "Enable menu debug logging");
    }
}