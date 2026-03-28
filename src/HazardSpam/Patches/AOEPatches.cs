using System.Reflection;
using ConsoleTools;
using HarmonyLib;
using HazardSpam.Hazards;
using UnityEngine;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(AOE))]
public static class AOEPatches
{
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
            __instance.fallTime = HazardTweaks.ExploSporeFallTime;
            __instance.knockback = HazardTweaks.ExploSporeKnockback;
            __instance.range = HazardTweaks.ExploSporeRange;
            __instance.statusAmount = HazardTweaks.ExploSporeStatusAmount;
            __instance.statusType = HazardTweaks.ExploSporeStatusType;
        }

        // PoisonSpore - initial explosion
        else if (__instance.gameObject.name.StartsWith("VFX_PoisonExplo"))
        {
            __instance.fallTime = HazardTweaks.PoisonSporeFallTime;
            __instance.knockback = HazardTweaks.PoisonSporeKnockback;
            if (HazardTweaks.PoisonSporeKnockback > 25)
                __instance.factorPow = DefaultHazardTweaks.ExploSpore_FactorPow;    // WARN: ugly hack
            __instance.range = HazardTweaks.PoisonSporeRange;
            __instance.statusAmount = HazardTweaks.PoisonSporeStatusAmount;
            __instance.statusType = HazardTweaks.PoisonSporeStatusType;
            
            // ===== ParticleSystem adjustments =====
            var ps = __instance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startLifetime = HazardTweaks.PoisonSporeRemoveAfterSeconds + 4;

                // Adjust Color over Lifetime to keep alpha longer
                var col = ps.colorOverLifetime;
                if (col.enabled)
                {
                    Gradient g = new Gradient();
                    g.SetKeys(
                        new GradientColorKey[] { 
                            new GradientColorKey(main.startColor.color, 0f), 
                            new GradientColorKey(main.startColor.color, 1f) 
                        },
                        new GradientAlphaKey[] { 
                            new GradientAlphaKey(1f, 0f),        // fully visible at start
                            new GradientAlphaKey(1f, 0.95f),     // stay visible until 95% of lifetime
                            new GradientAlphaKey(0f, 1f)         // fade at very end
                        }
                    );
                    col.color = g;
                }

                // Keep size constant if Size over Lifetime is enabled
                var size = ps.sizeOverLifetime;
                if (size.enabled)
                {
                    AnimationCurve curve = new AnimationCurve();
                    curve.AddKey(0f, 1f);   // start
                    curve.AddKey(1f, 1f);   // end
                    size.size = new ParticleSystem.MinMaxCurve(1f, curve);
                }
            }
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
            __instance.fallTime = HazardTweaks.FlashPlantFallTime;
            __instance.knockback = HazardTweaks.FlashPlantKnockback;
            if (HazardTweaks.FlashPlantKnockback > 25)
                __instance.factorPow = DefaultHazardTweaks.ExploSpore_FactorPow;    // WARN: ugly hack
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
            __instance.fallTime = HazardTweaks.GeyserFallTime;
            __instance.knockback = HazardTweaks.GeyserKnockback;
            __instance.range = HazardTweaks.GeyserRange;
            __instance.statusAmount = HazardTweaks.GeyserStatusAmount;
            __instance.statusType = HazardTweaks.GeyserStatusType;;
        }
        /*else
        {
            Plugin.Log.LogColorW($"Failed to patch AOE: {__instance.gameObject.name}");
        }*/
    }
}