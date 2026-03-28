using HarmonyLib;
using HazardSpam.Hazards;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(TimeEvent))]
public static class TimeEventPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TimeEvent.OnEnable))]
    private static void Prefix_TimeEvent_OnEnable(TimeEvent __instance)
    {
        // PoisonSpore - lingering cloud
        if (__instance.gameObject.name.StartsWith("VFX_SporePoisonExplo"))
        {
            __instance.rate = HazardTweaks.PoisonSporeRepeatRate;
        }
    }
}