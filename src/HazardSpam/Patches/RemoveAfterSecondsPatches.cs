using HarmonyLib;
using HazardSpam.Hazards;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(RemoveAfterSeconds))]
public static class RemoveAfterSecondsPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RemoveAfterSeconds.Start))]
    private static void Prefix_RemoveAfterSeconds_Start(RemoveAfterSeconds __instance)
    {
        // ExploSpore
        if (__instance.gameObject.name.StartsWith("VFX_SporeExploExplo"))
        {
            __instance.seconds = HazardTweaks.ExploSporeRemoveAfterSeconds;
        }
        
        // PoisonSpore - initial explosion
        else if (__instance.gameObject.name.StartsWith("VFX_PoisonExplo"))
        {
            __instance.seconds = HazardTweaks.PoisonSporeRemoveAfterSeconds + 4;
        }
        // PoisonSpore - lingering cloud
        else if (__instance.gameObject.name.StartsWith("VFX_SporePoisonExplo"))
        {
            __instance.seconds = HazardTweaks.PoisonSporeRemoveAfterSeconds;
        }
    }
}