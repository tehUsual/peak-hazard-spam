using HazardSpam.Menu.Descriptors;

namespace HazardSpam.Hazards;

public static class HazardTweaks
{
    // --- Jelly ---
    internal static float SlipperyJellyfishForceMultiplier = DefaultHazardTweaks.HS_SlipperyJellyfish_ForceMul;
    internal static float SlipperyJellyfishSpinMultiplier = DefaultHazardTweaks.HS_SlipperyJellyfish_SpinMul;
    internal static float SlipperyJellyfishStatusAmount = DefaultHazardTweaks.SlipperyJellyfish_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE SlipperyJellyfishStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.SlipperyJellyfish_StatusType;
    internal static float SlipperyJellyfishTriggerChance = DefaultHazardTweaks.HS_SlipperyJellyfish_TriggerChance;

    
    // --- Explo Spore ---
    internal static float ExploSporeTriggerChance = DefaultHazardTweaks.ExploSpore_TriggerChance;
    
    internal static float ExploSporeFallTime = DefaultHazardTweaks.ExploSpore_FallTime;
    internal static float ExploSporeKnockback = DefaultHazardTweaks.ExploSpore_Knockback;
    internal static float ExploSporeRange = DefaultHazardTweaks.ExploSpore_Range;
    internal static float ExploSporeStatusAmount = DefaultHazardTweaks.ExploSpore_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE ExploSporeStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.ExploSpore_StatusType;
    
    internal static float ExploSporeRemoveAfterSeconds = DefaultHazardTweaks.ExploSpore_RemoveAfterSeconds;

    
    // --- Poison Spore ---
    internal static float PoisonSporeTriggerChance = DefaultHazardTweaks.PoisonSpore_TriggerChance;
    internal static float PoisonSporeRepeatRate = DefaultHazardTweaks.PoisonSpore_RepeatRate;
    
    internal static float PoisonSporeFallTime = DefaultHazardTweaks.PoisonSpore_FallTime;
    internal static float PoisonSporeKnockback = DefaultHazardTweaks.PoisonSpore_Knockback;
    internal static float PoisonSporeRange = DefaultHazardTweaks.PoisonSpore_Range;
    internal static float PoisonSporeStatusAmount = DefaultHazardTweaks.PoisonSpore_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE PoisonSporeStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.PoisonSpore_StatusType;
    
    internal static float PoisonSporeRemoveAfterSeconds = DefaultHazardTweaks.PoisonSpore_RemoveAfterSeconds;
    
    
    // --- Urchin ---
    internal static float UrchinCooldown = DefaultHazardTweaks.Urchin_Cooldown;
    internal static float UrchinKnockback = DefaultHazardTweaks.Urchin_Knockback;
    internal static float UrchinStatusAmount = DefaultHazardTweaks.Urchin_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE UrchinStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.Urchin_StatusType;
    
    
    // --- Thorn ---
    internal static float ThornCooldown = DefaultHazardTweaks.Thorn_Cooldown;
    internal static float ThornKnockback = DefaultHazardTweaks.Thorn_Knockback;
    internal static float ThornStatusAmount = DefaultHazardTweaks.Thorn_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE ThornStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.Thorn_StatusType;
    
    
    // --- Poison Ivy ---
    internal static float PoisonIvyCooldown = DefaultHazardTweaks.PoisonIvy_Cooldown;
    internal static float PoisonIvyStatusAmount = DefaultHazardTweaks.PoisonIvy_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE PoisonIvyStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.PoisonIvy_StatusType;
    
    
    // --- Geyser ---
    internal static float GeyserTriggerChange = DefaultHazardTweaks.Geyser_TriggerChange;
    
    internal static float GeyserWarnAmount = DefaultHazardTweaks.Geyser_WarnAmount;
    internal static float GeyserWarnDuration = DefaultHazardTweaks.Geyser_WarnDuration;
    internal static float GeyserWarnRange = DefaultHazardTweaks.Geyser_WarnRange;
    internal static float GeyserWarnScale = DefaultHazardTweaks.Geyser_WarnScale;
    
    internal static float GeyserFallTime = DefaultHazardTweaks.Geyser_FallTime;
    internal static float GeyserKnockback = DefaultHazardTweaks.Geyser_Knockback;
    internal static float GeyserRange = DefaultHazardTweaks.Geyser_Range;
    internal static float GeyserStatusAmount = DefaultHazardTweaks.Geyser_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE GeyserStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.Geyser_StatusType;
    
    
    // --- Flash Plant ---
    internal static float FlashPlantTriggerChance = DefaultHazardTweaks.FlashPlant_TriggerChance;
    
    internal static float FlashPlantFallTime = DefaultHazardTweaks.FlashPlant_FallTime;
    internal static float FlashPlantKnockback = DefaultHazardTweaks.FlashPlant_Knockback;
    internal static float FlashPlantRange = DefaultHazardTweaks.FlashPlant_Range;
    internal static float FlashPlantStatusAmount = DefaultHazardTweaks.FlashPlant_StatusAmount;
    internal static CharacterAfflictions.STATUSTYPE FlashPlantStatusType = 
        (CharacterAfflictions.STATUSTYPE)DefaultHazardTweaks.FlashPlant_StatusType;
    
    //internal static float FlashPlantRemoveAfterSeconds = DefaultHazardTweaks.FlashPlant_RemoveAfterSeconds;
}