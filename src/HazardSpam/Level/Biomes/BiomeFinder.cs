using HazardSpam.Types;
using UnityEngine;

namespace HazardSpam.Level.Biomes;

public static class BiomeFinder
{

    public static BiomeInfo? FindActiveBiome(OurBiome biomeType)
    {
        switch (biomeType)
        {
            case OurBiome.Shore:
            {
                var go = GameObject.Find("Map/Biome_1/Beach/Beach_Segment");

                if (go == null)
                {
                    Plugin.Log.LogError($"Could not find Beach_Segment for Shore");
                    return null;
                }

                var variant =
                    DetermineBiomeVariant(System.Enum.GetNames(typeof(BiomeShoreVariants)), go.transform);

                if (variant == null)
                {
                    Plugin.Log.LogError($"Could not find biome variant for Shore");
                    return null;
                }

                return new BiomeInfo(biomeType, variant.name, go.transform, variant);
            }

            case OurBiome.Tropics:
            {
                var go = GameObject.Find("Map/Biome_2/Tropics/Jungle_Segment");

                if (go == null)
                {
                    Plugin.Log.LogError($"Could not find Jungle_Segment for Tropics");
                    return null;
                }

                var variant =
                    DetermineBiomeVariant(System.Enum.GetNames(typeof(BiomeTropicVariants)), go.transform);

                if (variant == null)
                {
                    Plugin.Log.LogError($"Could not find biome variant for Tropics");
                    return null;
                }

                return new BiomeInfo(biomeType, variant.name, go.transform, variant);
            }

            case OurBiome.Alpine:
            {
                var go = GameObject.Find("Map/Biome_3/Alpine/Snow_Segment");

                if (go == null)
                {
                    Plugin.Log.LogError($"Could not find Snow_Segment for Alpine");
                    return null;
                }

                var variant =
                    DetermineBiomeVariant(System.Enum.GetNames(typeof(BiomeAlpineVariants)), go.transform);

                if (variant == null)
                {
                    Plugin.Log.LogError($"Could not find biome variant for Alpine");
                    return null;
                }

                return new BiomeInfo(biomeType, variant.name, go.transform, variant);
            }

            case OurBiome.Mesa:
            {
                var go = GameObject.Find("Map/Biome_3/Mesa/Desert_Segment");

                if (go == null)
                {
                    Plugin.Log.LogError($"Could not find Desert_Segment for Mesa");
                    return null;
                }

                return new BiomeInfo(biomeType, "", go.transform, go.transform);
            }
        }

        return null;
    }

    private static Transform? DetermineBiomeVariant(string[] variants, Transform segment)
    {
        foreach (string variant in variants)
        {
            var go = segment.Find(variant);

            if (go == null)
            {
                Plugin.Log.LogWarning($"Biome variant '{variant}' is null");
                continue;
            }

            if (go.gameObject.activeSelf)
            {
                Plugin.Log.LogInfo($"Found biome variant '{variant}'");
                return go;
            }
        }
        return null;
    }
}