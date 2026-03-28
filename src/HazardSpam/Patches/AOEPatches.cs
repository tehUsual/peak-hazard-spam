using System.Reflection;
using ConsoleTools;
using HarmonyLib;
using HazardSpam.Hazards;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(AOE))]
public static class AOEPatches
{
    /*private static readonly FieldInfo HasStatusField =
        typeof(AOE).GetField("<hasStatus>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);*/
    
    /*
     * Should be okay to patch OnEnable for now as the timing allows for the AOE to be instantiated
     * AFTER the HazardTweaks are loaded.
     */
    [HarmonyPrefix]
    [HarmonyPatch(nameof(AOE.OnEnable))]
    private static void Prefix_AOE_OnEnable(AOE __instance)
    {
        // ExploSpore
        if (__instance.gameObject.name.StartsWith("VFX_SporeExploExplo"))
        {
            //HasStatusField.SetValue(__instance, HazardTweaks.ExploSporeHasStatus);
            __instance.fallTime = HazardTweaks.ExploSporeFallTime;
            __instance.knockback = HazardTweaks.ExploSporeKnockback;
            __instance.range = HazardTweaks.ExploSporeRange;
            __instance.statusAmount = HazardTweaks.ExploSporeStatusAmount;
            __instance.statusType = HazardTweaks.ExploSporeStatusType;
        }

        // PoisonSpore - initial explosion
        else if (__instance.gameObject.name.StartsWith("VFX_PoisonExplo"))
        {
            //HasStatusField.SetValue(__instance, HazardTweaks.PoisonSporeHasStatus);
            __instance.fallTime = HazardTweaks.PoisonSporeFallTime;
            __instance.knockback = HazardTweaks.PoisonSporeKnockback;
            __instance.range = HazardTweaks.PoisonSporeRange;
            __instance.statusAmount = HazardTweaks.PoisonSporeStatusAmount;
            __instance.statusType = HazardTweaks.PoisonSporeStatusType;
        }
        
        // PoisonSpore - lingering cloud
        else if (__instance.gameObject.name.StartsWith("VFX_SporePoisonExplo"))
        {
            __instance.range = HazardTweaks.PoisonSporeRange;
            __instance.statusAmount = HazardTweaks.PoisonSporeStatusAmount;
            __instance.statusType = HazardTweaks.PoisonSporeStatusType;
        }

        // FlashPlant
        else if (__instance.gameObject.name.StartsWith("Explosion") && __instance.illegalStatus == "BLIND")
        {
            //HasStatusField.SetValue(__instance, HazardTweaks.FlashPlantHasStatus);
            __instance.fallTime = HazardTweaks.FlashPlantFallTime;
            __instance.knockback = HazardTweaks.FlashPlantKnockback;
            __instance.range = HazardTweaks.FlashPlantRange;
            __instance.statusAmount = HazardTweaks.FlashPlantStatusAmount;
            __instance.statusType = HazardTweaks.FlashPlantStatusType;
        }

        // Geyser
        else if (
            __instance.gameObject.name.StartsWith("Explosion") &&
            __instance.transform.parent != null &&
            __instance.transform.parent.name.StartsWith("Geyser"))
        {
            //HasStatusField.SetValue(__instance, HazardTweaks.GeyserHasStatus);
            __instance.fallTime = HazardTweaks.GeyserFallTime;
            __instance.knockback = HazardTweaks.GeyserKnockback;
            __instance.range = HazardTweaks.GeyserRange;
            __instance.statusAmount = HazardTweaks.GeyserStatusAmount;
            __instance.statusType = HazardTweaks.GeyserStatusType;;
        }
        else
        {
            Plugin.Log.LogColorW($"Failed to patch AOE: {__instance.gameObject.name}");
        }
    }
}