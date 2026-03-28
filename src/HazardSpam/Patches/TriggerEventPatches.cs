using HarmonyLib;
using HazardSpam.Hazards;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(TriggerEvent))]
public static class TriggerEventPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(TriggerEvent.Start))]
    private static void Postfix_TriggerEvent_Start(TriggerEvent __instance)
    {
        // ExploSpore
        if (__instance.gameObject.name.StartsWith("Jungle_SporeMushroomExplo"))
        {
            __instance.triggerChance = HazardTweaks.ExploSporeTriggerChance;
        }

        // PoisonSpore
        else if (__instance.gameObject.name.StartsWith("Jungle_SporeMushroom"))
        {
            __instance.triggerChance = HazardTweaks.PoisonSporeTriggerChance;
        }
        
        // Flash Plant
        else if (__instance.gameObject.name.StartsWith("FlashPlant"))
        {
            __instance.triggerChance = HazardTweaks.FlashPlantTriggerChance;
        }

        // Geyser
        else if (__instance.gameObject.name.StartsWith("Geyser"))
        {
            __instance.triggerChance = HazardTweaks.GeyserTriggerChange;
        }
    }
}