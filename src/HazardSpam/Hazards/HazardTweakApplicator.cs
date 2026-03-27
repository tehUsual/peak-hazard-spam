using System.Net;
using ConsoleTools;
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
                    case HazardFields.Force:
                        HazardTweaks.SlipperyJellyfishForceMultiplier = (float)value / 100f;
                        break;
                    case HazardFields.Spin:
                        HazardTweaks.SlipperyJellyfishSpinMultiplier = (float)value / 100f;
                        break;
                    case HazardFields.Poison:
                        HazardTweaks.SlipperyJellyfishPoison = (float)value / 100f;
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
            Plugin.Log.LogColorW($"Failed to apply tweak for '{type}/{field}'. HazardType not found");
        } else if (!typeFound) {
            Plugin.Log.LogColorW($"Failed to apply tweak for '{type}/{field}'. Field not found");
        }
        else
        {
            Plugin.Log.LogColorS($"Successfully applied tweak for '{type}/{field}'");
        }
    }
}