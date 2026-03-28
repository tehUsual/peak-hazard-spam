using System.Net;
using ConsoleTools;
using HazardSpam.Menu.Descriptors;
using HazardSpam.Types;

namespace HazardSpam.Hazards;

public static class HazardTweakApplicator
{
    internal static void ApplyTweak(HazardType type, string field, object value)
    {
        bool fieldFound = true;
        bool typeFound = true;
        ;
        
        switch (type)
        {
            case HazardType.Jelly:
                switch (field)
                {
                    case TweakField.TriggerChance:
                        HazardTweaks.SlipperyJellyfishTriggerChance = (float)value / 100f;
                        break;
                    case TweakField.Force:
                        HazardTweaks.SlipperyJellyfishForceMultiplier = (float)value / 100f;
                        break;
                    case TweakField.Spin:
                        HazardTweaks.SlipperyJellyfishSpinMultiplier = (float)value / 100f;
                        break;
                    case TweakField.StatusAmount:
                        HazardTweaks.SlipperyJellyfishStatusAmount = (float)value / 100f;
                        break;
                    case TweakField.StatusType:
                        HazardTweaks.SlipperyJellyfishStatusType = (CharacterAfflictions.STATUSTYPE)value;
                        break;
                    default:
                        fieldFound = false;
                        break;
                }
                break;
            
            case HazardType.ExploSpore:
                switch (field)
                {
                    case TweakField.TriggerChance:
                        HazardTweaks.ExploSporeTriggerChance = (float)value / 100f;
                        break;
                    case TweakField.FallTime:
                        HazardTweaks.ExploSporeFallTime = (float)value;
                        break;
                    case TweakField.Knockback:
                        HazardTweaks.ExploSporeKnockback = (float)value;
                        break;
                    case TweakField.Range:
                        HazardTweaks.ExploSporeRange = (float)value;
                        break;
                    case TweakField.StatusAmount:
                        HazardTweaks.ExploSporeStatusAmount = (float)value / 100f;
                        break;
                    case TweakField.StatusType:
                        HazardTweaks.ExploSporeStatusType = (CharacterAfflictions.STATUSTYPE)value;
                        break;
                    default:
                        fieldFound = false;
                        break;
                }
                break;
            
            case HazardType.PoisonSpore:
                switch (field)
                {
                    case TweakField.TriggerChance:
                        HazardTweaks.PoisonSporeTriggerChance = (float)value / 100f;
                        break;
                    case TweakField.FallTime:
                        HazardTweaks.PoisonSporeFallTime = (float)value;
                        break;
                    case TweakField.Knockback:
                        HazardTweaks.PoisonSporeKnockback = (float)value;
                        break;
                    case TweakField.Range:
                        HazardTweaks.PoisonSporeRange = (float)value;
                        break;
                    case TweakField.RemoveAfterSeconds:
                        HazardTweaks.PoisonSporeRemoveAfterSeconds = (float)value;
                        break;
                    case TweakField.RepeatRate:
                        HazardTweaks.PoisonSporeRepeatRate = (float)value;
                        break;
                    case TweakField.StatusAmount:
                        HazardTweaks.PoisonSporeStatusAmount = (float)value / 100f;
                        break;
                    case TweakField.StatusType:
                        HazardTweaks.PoisonSporeStatusType = (CharacterAfflictions.STATUSTYPE)value;
                        break;
                    default:
                        fieldFound = false;
                        break;
                }
                break;
            
            case HazardType.Urchin:
                switch (field)
                {
                    case TweakField.Cooldown:
                        HazardTweaks.UrchinCooldown = (float)value;
                        break;
                    case TweakField.Knockback:
                        HazardTweaks.UrchinKnockback = (float)value;
                        break;
                    case TweakField.StatusAmount:
                        HazardTweaks.UrchinStatusAmount = (float)value / 100f;
                        break;
                    case TweakField.StatusType:
                        HazardTweaks.UrchinStatusType = (CharacterAfflictions.STATUSTYPE)value;
                        break;
                    default:
                        fieldFound = false;
                        break;
                }
                break;
            
            case HazardType.Thorn:
                switch (field)
                {
                    case TweakField.Cooldown:
                        HazardTweaks.ThornCooldown = (float)value;
                        break;
                    case TweakField.Knockback:
                        HazardTweaks.ThornKnockback = (float)value;
                        break;
                    case TweakField.StatusAmount:
                        HazardTweaks.ThornStatusAmount = (float)value / 100f;
                        break;
                    case TweakField.StatusType:
                        HazardTweaks.ThornStatusType = (CharacterAfflictions.STATUSTYPE)value;
                        break;
                    default:
                        fieldFound = false;
                        break;
                }
                break;
            
            case HazardType.PoisonIvy:
                switch (field)
                {
                    case TweakField.Cooldown:
                        HazardTweaks.PoisonIvyCooldown = (float)value;
                        break;
                    case TweakField.StatusAmount:
                        HazardTweaks.PoisonIvyStatusAmount = (float)value / 100f;
                        break;
                    case TweakField.StatusType:
                        HazardTweaks.PoisonIvyStatusType = (CharacterAfflictions.STATUSTYPE)value;
                        break;
                    default:
                        fieldFound = false;
                        break;
                }
                break;

            default:
                typeFound = false;
                break;
        }

        if (!fieldFound)
        {
            Plugin.Log.LogColorW($"Failed to apply tweak for '{type}/{field}'. Field not found");
        } else if (!typeFound) {
            Plugin.Log.LogColorW($"Failed to apply tweak for '{type}/{field}'. HazardType not found");
        }
        else
        {
            Plugin.Log.LogColorS($"Successfully applied tweak for '{type}/{field}'");
        }
    }
}