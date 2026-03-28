using System;
using System.Collections.Generic;
using System.Linq;
using HazardSpam.Hazards;
using HazardSpam.Types;

namespace HazardSpam.Menu.Descriptors;


public enum RestrictedAffliction
{
    Injury = CharacterAfflictions.STATUSTYPE.Injury,
    Hunger = CharacterAfflictions.STATUSTYPE.Hunger,
    Cold = CharacterAfflictions.STATUSTYPE.Cold,
    Poison = CharacterAfflictions.STATUSTYPE.Poison,
    Curse = CharacterAfflictions.STATUSTYPE.Curse,
    Drowsy = CharacterAfflictions.STATUSTYPE.Drowsy,
    Hot = CharacterAfflictions.STATUSTYPE.Hot,
    Thorns = CharacterAfflictions.STATUSTYPE.Thorns,
    Spores = CharacterAfflictions.STATUSTYPE.Spores
}


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

    /// <summary>When <see cref="Type"/> is <see cref="FieldType.Enum"/>, the enum type whose names populate the dropdown; stored value is <c>int</c>.</summary>
    public Type? EnumType { get; set; }

    public SpawnTypeField(string name, string displayName, FieldType type, object defaultValue,
        string description = "", float? minValue = null, float? maxValue = null, string unit = "", Type? enumType = null)
    {
        Name = name;
        DisplayName = displayName;
        Type = type;
        DefaultValue = defaultValue;
        Description = description;
        MinValue = minValue;
        MaxValue = maxValue;
        Unit = unit;
        EnumType = enumType;
    }
}

/// <summary>
/// Types of configurable fields
/// </summary>
public enum FieldType
{
    Float,
    Int,
    Bool,
    /// <summary>Dropdown of enum member names; persisted and exposed as <c>int</c>.</summary>
    Enum
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
                new SpawnTypeField(TweakField.TriggerChance, "Trigger Chance", FieldType.Float,
                    DefaultHazardTweaks.HS_SlipperyJellyfish_TriggerChance * 100,
                    "Probability of triggering", 0, 100, "%"),
                new SpawnTypeField(TweakField.Force, "Force", FieldType.Float,
                    DefaultHazardTweaks.HS_SlipperyJellyfish_ForceMul * 100,
                    "Force multiplier", null, null, "%"),
                new SpawnTypeField(TweakField.Spin, "SpinForce", FieldType.Float,
                    DefaultHazardTweaks.HS_SlipperyJellyfish_SpinMul * 100,
                    "Backflip multiplier (dizzy warning)", null, null, "%"),
                new SpawnTypeField(TweakField.StatusAmount, "Effect amount", FieldType.Float,
                    DefaultHazardTweaks.SlipperyJellyfish_StatusAmount * 100,
                    "Effect amount received on contact", 0, 100, "%"),
                new SpawnTypeField(TweakField.StatusType, "Effect Type", FieldType.Enum,
                    (int)DefaultHazardTweaks.SlipperyJellyfish_StatusType,
                    "Effect type received on contact", null, null, "", typeof(RestrictedAffliction)),
            }
        },
        
        new SpawnTypeDescriptor(HazardType.ExploSpore, "Explo Spore", "")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField(TweakField.TriggerChance, "Trigger Chance", FieldType.Float,
                    DefaultHazardTweaks.ExploSpore_TriggerChance * 100,
                    "Probability of triggering", 0, 100, "%"),
                new SpawnTypeField(TweakField.FallTime, "Fall time", FieldType.Float,
                    DefaultHazardTweaks.ExploSpore_FallTime,
                    "Time player falls over", 0, null, " sec"),
                new SpawnTypeField(TweakField.Knockback, "Knockback", FieldType.Float,
                    DefaultHazardTweaks.ExploSpore_Knockback,
                    "Knockback force applied to player", null, null, ""),
                new SpawnTypeField(TweakField.Range, "Range", FieldType.Float,
                    DefaultHazardTweaks.ExploSpore_Range,
                    "Range of explosion", 0, null, "m"),
                new SpawnTypeField(TweakField.StatusAmount, "Effect amount", FieldType.Float,
                    DefaultHazardTweaks.ExploSpore_StatusAmount * 100,
                    "Effect amount received on contact", 0, 100, "%"),
                new SpawnTypeField(TweakField.StatusType, "Effect Type", FieldType.Enum,
                    (int)DefaultHazardTweaks.ExploSpore_StatusType,
                    "Effect type received on contact", null, null, "", typeof(RestrictedAffliction)),
            }
        },
        
        new SpawnTypeDescriptor(HazardType.PoisonSpore, "Poison Spore", "")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField(TweakField.TriggerChance, "Trigger Chance", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_TriggerChance * 100,
                    "Probability of triggering", 0, 100, "%"),
                new SpawnTypeField(TweakField.FallTime, "Fall time", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_FallTime,
                    "Time player falls over", 0, null, " sec"),
                new SpawnTypeField(TweakField.Knockback, "Knockback", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_Knockback,
                    "Knockback force applied to player", null, null, ""),
                new SpawnTypeField(TweakField.Range, "Range", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_Range,
                    "Range of explosion", 0, null, "m"),
                new SpawnTypeField(TweakField.RemoveAfterSeconds, "Linger duration", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_RemoveAfterSeconds,
                    "Duration of lingering cloud", 0, null, " sec"),
                new SpawnTypeField(TweakField.RepeatRate, "Repeat Rate", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_RepeatRate,
                    "Rate at which cloud effect repeat", 0, null, " sec"),
                new SpawnTypeField(TweakField.StatusAmount, "Effect amount", FieldType.Float,
                    DefaultHazardTweaks.PoisonSpore_StatusAmount * 100,
                    "Effect amount received on contact", 0, 100, "%"),
                new SpawnTypeField(TweakField.StatusType, "Effect Type", FieldType.Enum,
                    (int)DefaultHazardTweaks.PoisonSpore_StatusType,
                    "Effect type received on contact", null, null, "", typeof(RestrictedAffliction)),
            }
        },
        
        new SpawnTypeDescriptor(HazardType.Urchin, "Urchin", "")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField(TweakField.Cooldown, "Cooldown", FieldType.Float,
                    DefaultHazardTweaks.Urchin_Cooldown,
                    "Probability of triggering", 0, null, " sec"),
                new SpawnTypeField(TweakField.Knockback, "Knockback", FieldType.Float,
                    DefaultHazardTweaks.Urchin_Knockback,
                    "Knockback force applied to player", null, null, ""),
                new SpawnTypeField(TweakField.StatusAmount, "Effect amount", FieldType.Float,
                    DefaultHazardTweaks.Urchin_StatusAmount * 100,
                    "Effect amount received on contact", 0, 100, "%"),
                new SpawnTypeField(TweakField.StatusType, "Effect Type", FieldType.Enum,
                    (int)DefaultHazardTweaks.Urchin_StatusType,
                    "Effect type received on contact", null, null, "", typeof(RestrictedAffliction)),
            }
        },
        
        new SpawnTypeDescriptor(HazardType.Thorn, "Thorns", "")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField(TweakField.Cooldown, "Cooldown", FieldType.Float,
                    DefaultHazardTweaks.Thorn_Cooldown,
                    "Probability of triggering", 0, null, " sec"),
                new SpawnTypeField(TweakField.Knockback, "Knockback", FieldType.Float,
                    DefaultHazardTweaks.Thorn_Knockback,
                    "Knockback force applied to player", null, null, ""),
                new SpawnTypeField(TweakField.StatusAmount, "Effect amount", FieldType.Float,
                    DefaultHazardTweaks.Thorn_StatusAmount * 100,
                    "Effect amount received on contact", 0, 100, "%"),
                new SpawnTypeField(TweakField.StatusType, "Effect Type", FieldType.Enum,
                    (int)DefaultHazardTweaks.Thorn_StatusType,
                    "Effect type received on contact", null, null, "", typeof(RestrictedAffliction)),
            }
        },
        
        new SpawnTypeDescriptor(HazardType.PoisonIvy, "Poison Ivy", "")
        {
            Fields = new List<SpawnTypeField>
            {
                new SpawnTypeField(TweakField.Cooldown, "Cooldown", FieldType.Float,
                    DefaultHazardTweaks.PoisonIvy_Cooldown,
                    "Probability of triggering", 0, null, " sec"),
                new SpawnTypeField(TweakField.StatusAmount, "Effect amount", FieldType.Float,
                    DefaultHazardTweaks.PoisonIvy_StatusAmount * 100,
                    "Effect amount received on contact", 0, 100, "%"),
                new SpawnTypeField(TweakField.StatusType, "Effect Type", FieldType.Enum,
                    (int)DefaultHazardTweaks.PoisonIvy_StatusType,
                    "Effect type received on contact", null, null, "", typeof(RestrictedAffliction)),
            }
        },
        
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
