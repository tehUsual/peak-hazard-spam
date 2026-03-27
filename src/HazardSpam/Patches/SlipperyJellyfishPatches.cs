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
    private static readonly FieldInfo PoisonField =
        AccessTools.Field(typeof(HazardTweaks), nameof(HazardTweaks.SlipperyJellyfishPoison));
    
    
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(SlipperyJellyfish.Trigger))]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            // Replace poison value
            ;
            if (code.opcode == OpCodes.Ldc_R4 && 
                Mathf.Approximately((float)code.operand, DefaultHazardTweaks.SlipperyJellyfish_Poison))
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
}