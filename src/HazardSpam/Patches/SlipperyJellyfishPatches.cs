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
            
            // Replace status type
            if (TryGetInt(code, out int val) &&
                val == (int)DefaultHazardTweaks.SlipperyJellyfish_StatusType)
            {
                yield return new CodeInstruction(OpCodes.Ldsfld, StatusTypeField);
                continue;
            }

            yield return code;
        }
    }
    
    private static bool TryGetInt(CodeInstruction code, out int value)
    {
        switch (code.opcode.Name)
        {
            case "ldc.i4.m1": value = -1; return true;
            case "ldc.i4.0": value = 0; return true;
            case "ldc.i4.1": value = 1; return true;
            case "ldc.i4.2": value = 2; return true;
            case "ldc.i4.3": value = 3; return true;
            case "ldc.i4.4": value = 4; return true;
            case "ldc.i4.5": value = 5; return true;
            case "ldc.i4.6": value = 6; return true;
            case "ldc.i4.7": value = 7; return true;
            case "ldc.i4.8": value = 8; return true;
        }

        if (code.opcode == OpCodes.Ldc_I4)
        {
            value = (int)code.operand;
            return true;
        }

        if (code.opcode == OpCodes.Ldc_I4_S)
        {
            value = (sbyte)code.operand;
            return true;
        }

        value = 0;
        return false;
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