using System.Collections.Generic;
using System.Collections;
using System.Linq;
using HazardSpam.Config;
using HazardSpam.Level.Biomes;
using HazardSpam.Level.Caldera;
using HazardSpam.Networking;
using HazardSpam.Spawning;
using HazardSpam.StaticData;
using HazardSpam.Types;
using UnityEngine;

namespace HazardSpam.Level;

public static class LevelManager
{
    // Shore    { "Default", "SnakeBeach", "RedBeach", "BlueBeach", "JellyHell", "BlackSand" };
    // Tropics  { "Default", "Lava", "Pillars", "Thorny", "Bombs", "Ivy", "SkyJungle" };
    // Apline   { "Default", "Lava", "Spiky", "GeyserHell" };

    public static List<BiomeInfo> BiomeInfo { get; } = [];
    public static BiomeInfo? GetShoreBiome() => BiomeInfo.FirstOrDefault(b => b.BiomeType == Biome.BiomeType.Shore);
    public static BiomeInfo? GetTropicBiome() => BiomeInfo.FirstOrDefault(b => b.BiomeType == Biome.BiomeType.Tropics);
    public static BiomeInfo? GetAlpineBiome() => BiomeInfo.FirstOrDefault(b => b.BiomeType == Biome.BiomeType.Alpine);
    public static BiomeInfo? GetMesaBiome() => BiomeInfo.FirstOrDefault(b => b.BiomeType == Biome.BiomeType.Mesa);

    public static BiomeInfo? GetBiomeInfo(Biome.BiomeType biomeType) => BiomeInfo.FirstOrDefault(b => b.BiomeType == biomeType);
    
    
    public static void Init(List<Biome.BiomeType> currentBiomeTypes)
    {
        Reset();
        GatherBiomesInfo(currentBiomeTypes);
    }

    public static void Reset()
    {
        foreach (var biomeInfo in BiomeInfo)
        {
            biomeInfo.Clear();
        }
        BiomeInfo.Clear();
    }
    
    private static void GatherBiomesInfo(List<Biome.BiomeType> biomeTypes)
    {
        Plugin.Log.LogInfo($"Gathering info for BiomeTypes: {string.Join(", ", LevelState.GetBiomeTypes())}");
        foreach (Biome.BiomeType biomeType in biomeTypes)
        {
            if (biomeType == Biome.BiomeType.Volcano)
                continue;
            
            BiomeInfo? biomeInfo = BiomeFinder.FindActiveBiome(biomeType);
            if (biomeInfo == null)
            {
                Plugin.Log.LogError($"Failed to gather biome {biomeType}");
                continue;
            }
            BiomeInfo.Add(biomeInfo);
        }
        
        foreach (var biomeInfo in BiomeInfo)
        {
            Plugin.Log.LogInfo(
                $"Successfully gathered spawners for {biomeInfo.BiomeType.ToString()}/{biomeInfo.BiomeVariant}");
        }
    }
    
    
    public static void InitBiome(Biome.BiomeType biomeType, float intervalDelay = 0.15f)
    {
        if (biomeType == Biome.BiomeType.Volcano)
            CalderaHandler.InitCaldera();
        else
            SpawnerNetwork.Instance.StartCoroutine(InvokeInitBiome(biomeType, intervalDelay));
    }

    private static IEnumerator InvokeInitBiome(Biome.BiomeType biomeType, float intervalDelay)
    {
        BiomeInfo? biomeInfo = GetBiomeInfo(biomeType);
        if (biomeInfo == null)
        {
            Plugin.Log.LogError($"Failed to init {biomeType}");
            yield break;
        }

        foreach (var spawner in biomeInfo.Spawners)
        {
            int spawnCount = ConfigHandler.GetSpawnRate(biomeType, spawner.Area, spawner.SpawnType);
            if (spawnCount == 0)
            {
                Plugin.Log.LogWarning(
                    $"Unknown spawn rate for {spawner.BiomeType.ToString()}/{spawner.Area.ToString()}/{spawner.SpawnType.ToString()}");
                continue;
            }

            var (pos, rot, scaleGain) = spawner.GenerateSpawnPoints(spawnCount);
            if (pos.Length > 0)
            {
                // Dirty, add delay for shore to ensure clients also get the props
                if (biomeInfo.BiomeType == Biome.BiomeType.Shore)
                {
                    Plugin.Log.LogInfo($"CRUDE FIX: Delaying shore spawn for 12 seconds");   
                    yield return new WaitForSeconds(12f);
                }

                //SpawnerNetwork.Instance.SpawnPropsNetwork(spawner, pos, rot, scaleGain);
                SpawnerNetwork.Instance.SpawnPropsNetwork(spawner, pos, rot, scaleGain);
            }

            yield return new WaitForSeconds(intervalDelay);
        }
    }
    
    public static void ClearBiome(Biome.BiomeType biomeType, float intervalDelay = 0.15f)
    {
        SpawnerNetwork.Instance.StartCoroutine(InvokeClearBiome(biomeType, intervalDelay));
    }

    private static IEnumerator InvokeClearBiome(Biome.BiomeType biomeType, float intervalDelay)
    {
        BiomeInfo? biomeInfo = GetBiomeInfo(biomeType);
        if (biomeInfo == null)
        {
            Plugin.Log.LogError($"Failed to clear {biomeType}");    // shit
            yield break;
        }

        foreach (var spawner in biomeInfo.Spawners)
        {
            SpawnerNetwork.Instance.ClearPropsNetwork(spawner);
            yield return new WaitForSeconds(intervalDelay);
        }
    }

    public static string GenSpawnID(Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType, int index)
    {
        return $"{biomeType.ToString()}-{area.ToString()}-{spawnType.ToString()}-{index}";
    }
}