using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HazardSpam.Level;

public static class LevelState
{
    private static MapHandler? _mapHandlerInstance;
    public static MapHandler MapHandlerInstance =>
        _mapHandlerInstance ?? throw new InvalidOperationException("MapHandler not set yet.");

    private static bool _triedToFetchMapHandler = false;
    
    // Segments
    // { Beach, Tropics, Alpine, Caldera, TheKiln, Peak }
    private static Biome.BiomeType _currentBiome = Biome.BiomeType.Shore;
    // BiomeType
    //{ Shore = 0, Tropics = 1, Alpine = 2, Volcano = 3, Peak = 5, Mesa = 6, Colony = 7 }
    
    public static event Action<Biome.BiomeType>? OnBiomeLoading;
    public static event Action<Biome.BiomeType>? OnBiomeComplete;
    
    public static void Initialize(MapHandler instance)
    {
        if (_mapHandlerInstance != null) return;
        _mapHandlerInstance = instance;
    }

    public static List<Biome.BiomeType> GetBiomeTypes()
    {
        if (_mapHandlerInstance == null) 
        {
            Plugin.Log.LogError("[LevelState] MapHandler is null, could not trigger GetBiomeTypes()");
            return new List<Biome.BiomeType>();
        }
        Plugin.Log.LogInfo($"[LevelState] Got biome types: {string.Join(", ", _mapHandlerInstance.biomes)}");
        return _mapHandlerInstance.biomes;
    }

    public static void SetBiomeLoading(Biome.BiomeType biomeType)
    {
        Plugin.Log.LogInfo($"[LevelState] Biome loading: {biomeType}");
        _currentBiome = biomeType;
        if (_mapHandlerInstance != null)
            _mapHandlerInstance.StartCoroutine(InvokeOnBiomeLoading(_currentBiome));
        else
            Plugin.Log.LogError($"[LevelState] MapHandler is null, did not trigger SetBiomeLoading() for {biomeType}");
    }

    public static void SetBiomeLoading()
    {
        if (_mapHandlerInstance == null && !_triedToFetchMapHandler)
        {
            TryFetchMapHandler();
            _triedToFetchMapHandler = true;
        }

        if (_mapHandlerInstance != null)
        {
            Biome.BiomeType biomeType = _mapHandlerInstance.GetCurrentBiome();
            Segment segment = _mapHandlerInstance.GetCurrentSegment();
            
            // lazy
            if (segment == Segment.TheKiln)
                SetBiomeLoading(Biome.BiomeType.Colony);
            else
                SetBiomeLoading(biomeType);
        }
        else
            Plugin.Log.LogError("[LevelState] MapHandler is null, did not trigger SetBiomeLoading()");
    }

    public static bool TryFetchMapHandler()
    {
        var go = GameObject.Find("Map");
        if (go == null) return false;
        
        var mapHandler = go.GetComponent<MapHandler>();
        if (mapHandler == null) return false;
        
        Initialize(mapHandler);
        return true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator InvokeOnBiomeLoading(Biome.BiomeType biomeType)
    {
        yield return null;
        OnBiomeLoading?.Invoke(biomeType);
    }

    public static void SetBiomeComplete(Biome.BiomeType biomeType)
    {
        Plugin.Log.LogInfo($"[LevelState] Segment complete: {biomeType}");
        if (_mapHandlerInstance != null)
            _mapHandlerInstance.StartCoroutine(InvokeOnBiomeComplete(biomeType));
        else
            Plugin.Log.LogError($"[LevelState] MapHandler is null, did not trigger SetBiomeLoading() for {biomeType}");
    }

    public static void SetBiomeComplete()
    {
        if (_mapHandlerInstance != null)
        {
            Biome.BiomeType biomeType = _mapHandlerInstance.GetCurrentBiome();
            Segment segment = _mapHandlerInstance.GetCurrentSegment();
            
            // lazy
            if (segment == Segment.TheKiln)
                SetBiomeComplete(Biome.BiomeType.Colony);
            else
                SetBiomeComplete(biomeType);
        }
        else
            Plugin.Log.LogError("[LevelState] MapHandler is null, did not trigger SetBiomeComplete()");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator InvokeOnBiomeComplete(Biome.BiomeType biomeType)
    {
        yield return null;
        OnBiomeComplete?.Invoke(biomeType);
    }
}