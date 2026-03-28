using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using HazardSpam.Hazards;
using System.Reflection.Emit;
using UnityEngine;

namespace HazardSpam.Patches;

[HarmonyPatch(typeof(SlipperyJellyfish))]
public static class SlipperyJellyfishPatches
{
    // Slip force
    private static readonly FieldInfo ForceMultiplierField =
        AccessTools.Field(typeof(HazardTweaks), nameof(HazardTweaks.SlipperyJellyfishForceMultiplier));
    private static readonly FieldInfo SpinMultiplierField =
        AccessTools.Field(typeof(HazardTweaks), nameof(HazardTweaks.SlipperyJellyfishSpinMultiplier));
    // Poison
    private static readonly FieldInfo StatusTypeField =
        AccessTools.Field(typeof(HazardTweaks), nameof(HazardTweaks.SlipperyJellyfishStatusType));
    private static readonly FieldInfo PoisonField =
        AccessTools.Field(typeof(HazardTweaks), nameof(HazardTweaks.SlipperyJellyfishStatusAmount));
    
    
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(SlipperyJellyfish.Trigger))]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            // Replace status type
            if ((code.opcode == OpCodes.Ldc_I4 || code.opcode == OpCodes.Ldc_I4_S) &&
                (int)code.operand == (int)DefaultHazardTweaks.SlipperyJellyfish_StatusType)
            {
                yield return new CodeInstruction(OpCodes.Ldsfld, StatusTypeField);
                continue;
            }

            // Replace status amount
            if (code.opcode == OpCodes.Ldc_R4 && 
                Mathf.Approximately((float)code.operand, DefaultHazardTweaks.SlipperyJellyfish_StatusAmount))
            {
                yield return new CodeInstruction(OpCodes.Ldsfld, PoisonField);
                continue;
            }
            
            // Multiply force constants
            if (code.opcode == OpCodes.Ldc_R4)
            {
                if (Mathf.Approximately((float)code.operand, DefaultHazardTweaks.SlipperyJellyfish_ForceFeet) ||
                    Mathf.Approximately((float)code.operand, DefaultHazardTweaks.SlipperyJellyfish_ForceHip))
                {
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ldsfld, ForceMultiplierField);
                    yield return new CodeInstruction(OpCodes.Mul);
                    continue;
                } 
                if (Mathf.Approximately((float)code.operand, DefaultHazardTweaks.SlipperyJellyfish_ForceHead))
                {
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ldsfld, SpinMultiplierField);
                    yield return new CodeInstruction(OpCodes.Mul);
                    continue;
                }
            }

            yield return code;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SlipperyJellyfish.OnTriggerEnter))]
    private static bool Prefix_SlipperyJellyfish_OnTriggerEnter(SlipperyJellyfish __instance)
    {
        if (HazardTweaks.SlipperyJellyfishTriggerChance < Random.value) 
            return false;   // cancel event
        return true;    // allow event
    }
}