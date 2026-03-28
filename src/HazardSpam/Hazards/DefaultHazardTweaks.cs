using HazardSpam.Menu.Descriptors;

namespace HazardSpam.Hazards;

public static class DefaultHazardTweaks
{
    // --- Jelly ---
    public static readonly float SlipperyJellyfish_ForceFeet = 200f;
    public static readonly float SlipperyJellyfish_ForceHip = 1500f;
    public static readonly float SlipperyJellyfish_ForceHead = -300f;
    public static readonly float SlipperyJellyfish_StatusAmount = 0.05f;
    public static readonly RestrictedAffliction SlipperyJellyfish_StatusType = RestrictedAffliction.Poison;

    public static readonly float HS_SlipperyJellyfish_ForceMul = 1f;
    public static readonly float HS_SlipperyJellyfish_SpinMul = 1f;
    public static readonly float HS_SlipperyJellyfish_TriggerChance = 1f;

    
    // --- Explo Spore ---
    public static readonly float ExploSpore_TriggerChance = 1f;
    
    public static readonly float ExploSpore_FactorPow = 1.0f;
    public static readonly float ExploSpore_FallTime = 1f;
    public static readonly float ExploSpore_Knockback = 1300f;
    public static readonly float ExploSpore_Range = 6f;
    public static readonly float ExploSpore_StatusAmount = 0f;
    public static readonly RestrictedAffliction ExploSpore_StatusType = RestrictedAffliction.Poison;
    
    public static readonly float ExploSpore_RemoveAfterSeconds = 5f;
    
    
    // --- Poison Spore ---
    public static readonly float PoisonSpore_TriggerChance = 1f;
    public static readonly float PoisonSpore_RepeatRate = 0.5f;
    
    public static readonly float PoisonSpore_FactorPow = 0.5f;
    public static readonly float PoisonSpore_FallTime = 0f;
    public static readonly float PoisonSpore_Knockback = 0f;
    public static readonly float PoisonSpore_Range = 10f;
    public static readonly float PoisonSpore_StatusAmount = 0.2f;
    public static readonly RestrictedAffliction PoisonSpore_StatusType = RestrictedAffliction.Poison;

    public static readonly float PoisonSpore_RemoveAfterSeconds = 11f;
    
    
    // --- Urchin ---
    public static readonly float Urchin_Cooldown = 1f;
    public static readonly float Urchin_Knockback = 20f;
    public static readonly float Urchin_StatusAmount = 0.1f;
    public static readonly RestrictedAffliction Urchin_StatusType = RestrictedAffliction.Poison;
    
    
    // --- Thorn ---
    public static readonly float Thorn_Cooldown = 1f;
    public static readonly float Thorn_Knockback = 1000f;
    public static readonly float Thorn_StatusAmount = 0.1f;
    public static readonly RestrictedAffliction Thorn_StatusType = RestrictedAffliction.Poison;
    
    
    // --- Poison Ivy ---
    public static readonly float PoisonIvy_Cooldown = 1f;
    public static readonly float PoisonIvy_StatusAmount = 0.035f;
    public static readonly RestrictedAffliction PoisonIvy_StatusType = RestrictedAffliction.Poison;
    
    
    // --- Geyser ---
    public static readonly float Geyser_TriggerChange = 0.333f;

    public static readonly float Geyser_WarnAmount = 8f;
    public static readonly float Geyser_WarnDuration = 3f;
    public static readonly float Geyser_WarnRange = 15f;
    public static readonly float Geyser_WarnScale = 12f;
    
    public static readonly float Geyser_FactorPow = 0.5f;
    public static readonly float Geyser_FallTime = 1f;
    public static readonly float Geyser_Knockback = 1500f;
    public static readonly float Geyser_Range = 5f;
    public static readonly float Geyser_StatusAmount = 0.3f;
    public static readonly RestrictedAffliction Geyser_StatusType = RestrictedAffliction.Hot;
    
    
    // --- Flash Plant ---
    public static readonly float FlashPlant_TriggerChance = 1f;
    
    public static readonly float FlashPlant_FactorPow = 0.3f;
    public static readonly float FlashPlant_FallTime = 0f;
    public static readonly float FlashPlant_Knockback = 0f;
    public static readonly float FlashPlant_Range = 4f;
    public static readonly float FlashPlant_StatusAmount = 45f;
    public static readonly RestrictedAffliction FlashPlant_StatusType = RestrictedAffliction.Poison;
    
    //public static readonly float FlashPlant_RemoveAfterSeconds = 15f;
}