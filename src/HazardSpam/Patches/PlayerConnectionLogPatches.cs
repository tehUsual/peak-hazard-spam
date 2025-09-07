using HarmonyLib;
using HazardSpam.Util;

namespace HazardSpam.Patches;

[HarmonyPatch]
public static class PlayerConnectionLogPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerConnectionLog), "Awake")]
    public static void PostfixAwake(PlayerConnectionLog __instance)
    {
        StatusMessageHandler.Init(__instance);
    }
}