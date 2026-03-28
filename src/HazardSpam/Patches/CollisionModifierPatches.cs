using HarmonyLib;
using HazardSpam.Hazards;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(CollisionModifier))]
public class CollisionModifierPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CollisionModifier.Awake))]
    private static void Postfix_CollisionModifier_Awake(CollisionModifier __instance)
    {
        // Urchin
        if (__instance.transform.parent.name.StartsWith("Urch"))
        {
            __instance.cooldown = HazardTweaks.UrchinCooldown;
            __instance.damage = HazardTweaks.UrchinStatusAmount;
            __instance.knockback = HazardTweaks.UrchinKnockback;
            __instance.statusType = HazardTweaks.UrchinStatusType;
        }
        
        // Thorn
        else if (__instance.gameObject.name.StartsWith("JungleThornBase"))
        {
            __instance.cooldown = HazardTweaks.ThornCooldown;
            __instance.damage = HazardTweaks.ThornStatusAmount;
            __instance.knockback = HazardTweaks.ThornKnockback;
            __instance.statusType = HazardTweaks.ThornStatusType;
        }
    }
}