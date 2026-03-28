using ConsoleTools;
using HarmonyLib;
using HazardSpam.Hazards;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(StatusTrigger))]
public class StatusTriggerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StatusTrigger.OnTriggerEnter))]
    private static void Prefix_StatusTrigger_OnTriggerEnter(StatusTrigger __instance)
    {
        // Poison Ivy
        if (__instance.transform.parent.name.Contains("PoisonIvy"))
        {
            __instance.cooldown = HazardTweaks.PoisonIvyCooldown;
            __instance.statusAmount = HazardTweaks.PoisonIvyStatusAmount;
            __instance.statusType = HazardTweaks.PoisonIvyStatusType;
            Plugin.Log.LogColor($"Poison Ivy triggered with status type: {HazardTweaks.PoisonIvyStatusAmount.ToString()}");
        }
    }
}