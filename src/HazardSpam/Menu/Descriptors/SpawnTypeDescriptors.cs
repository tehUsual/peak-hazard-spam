using System.Collections.Generic;
using System.Linq;
using HazardSpam.Hazards;
using HazardSpam.Types;

namespace HazardSpam.Menu.Descriptors;

/// <summary>
/// Describes a configurable field for a SpawnType
/// </summary>
public class SpawnTypeField
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public object DefaultValue { get; set; } = 0f;
    public float? MinValue { get; set; }
    public float? MaxValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    
    public SpawnTypeField(string name, string displayName, FieldType type, object defaultValue, 
        string description = "", float? minValue = null, float? maxValue = null, string unit = "")
    {
        Name = name;
        DisplayName = displayName;
        Type = type;
        DefaultValue = defaultValue;
        Description = description;
        MinValue = minValue;
        MaxValue = maxValue;
        Unit = unit;
    }
}

/// <summary>
/// Types of configurable fields
/// </summary>
public enum FieldType
{
    Float,
    Int,
    Bool
}

/// <summary>
/// Describes a SpawnType and its configurable fields
/// </summary>
public class SpawnTypeDescriptor
{
    public HazardType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<SpawnTypeField> Fields { get; set; } = new();
    
    public SpawnTypeDescriptor(HazardType type, string displayName, string description = "")
    {
        Type = type;
        DisplayName = displayName;
        Description = description;
    }
}

/// <summary>
/// Static registry of SpawnType descriptors for Hazard Config
/// </summary>
public static class SpawnTypeDescriptors
{
    private static readonly List<SpawnTypeDescriptor> _descriptors = new()
    {
        new SpawnTypeDescriptor(HazardType.Jelly, "Jelly", "")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField(HazardFields.Force, "Force", FieldType.Float,
                    DefaultHazardTweaks.HS_SlipperyJellyfish_ForceMul * 100,
                    "Force multiplier", null, null, "%"),
                new SpawnTypeField(HazardFields.Spin, "SpinForce", FieldType.Float,
                    DefaultHazardTweaks.HS_SlipperyJellyfish_SpinMul * 100,
                    "Backflip multiplier (dizzy warning)", null, null, "%"),
                new SpawnTypeField(HazardFields.Poison, "Poison", FieldType.Float,
                    DefaultHazardTweaks.SlipperyJellyfish_Poison * 100,
                    "Poison received on contact", 0, 100, "%"),
            }
        },
        
        new SpawnTypeDescriptor(HazardType.Geyser, "Geyser", "Volcanic geyser that erupts periodically")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField("triggerChance", "Trigger Chance", FieldType.Float, 0.3f, 
                    "Probability of geyser triggering (0.0-1.0)", 0f, 1f, "%"),
                new SpawnTypeField("range", "Range", FieldType.Float, 5f, 
                    "Explosion radius in meters", 1f, 20f, "m"),
                new SpawnTypeField("damage", "Damage", FieldType.Float, 25f, 
                    "Damage dealt to player", 1f, 100f, "HP"),
                new SpawnTypeField("knockback", "Knockback", FieldType.Float, 8f, 
                    "Knockback force applied to player", 0f, 50f, "N")
            }
        },
        new SpawnTypeDescriptor(HazardType.Dynamite, "Dynamite", "Explosive that detonates on contact")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField("damage", "Damage", FieldType.Float, 40f, 
                    "Damage dealt to player", 1f, 200f, "HP"),
                new SpawnTypeField("range", "Explosion Range", FieldType.Float, 8f, 
                    "Explosion radius in meters", 1f, 30f, "m"),
                new SpawnTypeField("knockback", "Knockback", FieldType.Float, 15f, 
                    "Knockback force applied to player", 0f, 100f, "N"),
                new SpawnTypeField("fuseTime", "Fuse Time", FieldType.Float, 2f, 
                    "Time before explosion in seconds", 0.5f, 10f, "s")
            }
        },
        
        new SpawnTypeDescriptor(HazardType.Cactus, "Cactus", "Spiky plant that damages on contact")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField("damage", "Damage", FieldType.Float, 10f, 
                    "Damage dealt to player", 1f, 50f, "HP"),
                new SpawnTypeField("knockback", "Knockback", FieldType.Float, 3f, 
                    "Knockback force applied to player", 0f, 25f, "N"),
                new SpawnTypeField("spikeLength", "Spike Length", FieldType.Float, 1.5f, 
                    "Length of cactus spikes", 0.5f, 5f, "m")
            }
        },
        
        new SpawnTypeDescriptor(HazardType.Urchin, "Urchin", "Sea urchin with sharp spines")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField("damage", "Damage", FieldType.Float, 8f, 
                    "Damage dealt to player", 1f, 30f, "HP"),
                new SpawnTypeField("knockback", "Knockback", FieldType.Float, 2f, 
                    "Knockback force applied to player", 0f, 15f, "N"),
                new SpawnTypeField("spineCount", "Spine Count", FieldType.Int, 12, 
                    "Number of spines on the urchin", 3, 50, "spines")
            }
        }
    };
    
    /// <summary>
    /// Get all SpawnType descriptors
    /// </summary>
    public static IEnumerable<SpawnTypeDescriptor> GetAll()
    {
        return _descriptors.AsReadOnly();
    }
    
    /// <summary>
    /// Get a SpawnType descriptor by type
    /// </summary>
    public static SpawnTypeDescriptor? GetByType(HazardType type)
    {
        return _descriptors.FirstOrDefault(d => d.Type == type);
    }
    
    /// <summary>
    /// Get all SpawnTypes that have configurable fields
    /// </summary>
    public static IEnumerable<SpawnTypeDescriptor> GetConfigurableTypes()
    {
        return _descriptors.Where(d => d.Fields.Count > 0);
    }
}
