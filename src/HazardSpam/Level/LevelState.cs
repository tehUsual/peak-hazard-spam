using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HazardSpam.Level;

// Game does not define Caldera in its Biome enum. WTF!?
public enum OurBiome
{
    Shore = 0,
    Tropics = 1,
    Roots = 2,
    Alpine = 3,
    Mesa = 4,
    Caldera = 5,
    Kiln = 6,
    Peak = 7,
}

public static class BiomeConv
{
    public static OurBiome FromSegmentBiome(Segment s, Biome.BiomeType b)
    {
        switch (s)
        {
            case Segment.Caldera:
                return OurBiome.Caldera;
            case Segment.TheKiln:
                return OurBiome.Kiln;
            case Segment.Peak:
                return OurBiome.Peak;
            case Segment.Tropics:
                if (b == Biome.BiomeType.Roots) return OurBiome.Roots;
                else return OurBiome.Tropics;
            case Segment.Alpine:
                if (b == Biome.BiomeType.Alpine) return OurBiome.Alpine;
                else return OurBiome.Mesa;
            case Segment.Beach:
            default:
                return OurBiome.Shore;
        }
    }
}


public static class LevelState
{
    private static MapHandler? _mapHandlerInstance;
    public static MapHandler MapHandlerInstance =>
        _mapHandlerInstance ?? throw new InvalidOperationException("MapHandler not set yet.");

    private static bool _triedToFetchMapHandler = false;

    private static OurBiome _currentBiome = OurBiome.Shore;

    public static event Action<OurBiome>? OnBiomeLoading;
    public static event Action<OurBiome>? OnBiomeComplete;

    public static void Initialize(MapHandler instance)
    {
        if (_mapHandlerInstance != null) return;
        _mapHandlerInstance = instance;
    }

    public static List<OurBiome> GetBiomeTypes()
    {
        if (_mapHandlerInstance == null)
        {
            Plugin.Log.LogError("[LevelState] MapHandler is null, could not trigger GetBiomeTypes()");
            return new List<OurBiome>();
        }
        Plugin.Log.LogInfo($"[LevelState] Got biome types: {string.Join(", ", _mapHandlerInstance.biomes)}");
        return _mapHandlerInstance.segments.Select((s, i) => BiomeConv.FromSegmentBiome((Segment) i, s.biome)).Distinct().ToList();
    }

    public static void SetBiomeLoading(OurBiome b)
    {
        _currentBiome = b;
        Plugin.Log.LogInfo($"[LevelState] Biome loading: {_currentBiome}");
        if (_mapHandlerInstance != null)
            _mapHandlerInstance.StartCoroutine(InvokeOnBiomeLoading(_currentBiome));
        else
            Plugin.Log.LogError($"[LevelState] MapHandler is null, did not trigger SetBiomeLoading() for {_currentBiome}");
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
            SetBiomeLoading(BiomeConv.FromSegmentBiome(segment, biomeType));
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
    private static IEnumerator InvokeOnBiomeLoading(OurBiome biomeType)
    {
        yield return null;
        OnBiomeLoading?.Invoke(biomeType);
    }

    public static void SetBiomeComplete(OurBiome b)
    {
        Plugin.Log.LogInfo($"[LevelState] Segment complete: {b}");
        if (_mapHandlerInstance != null)
            _mapHandlerInstance.StartCoroutine(InvokeOnBiomeComplete(b));
        else
            Plugin.Log.LogError($"[LevelState] MapHandler is null, did not trigger SetBiomeLoading() for {b}");
    }

    public static void SetBiomeComplete()
    {
        if (_mapHandlerInstance != null)
        {
            Biome.BiomeType biomeType = _mapHandlerInstance.GetCurrentBiome();
            Segment segment = _mapHandlerInstance.GetCurrentSegment();
            SetBiomeComplete(BiomeConv.FromSegmentBiome(segment, biomeType));
        }
        else
            Plugin.Log.LogError("[LevelState] MapHandler is null, did not trigger SetBiomeComplete()");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator InvokeOnBiomeComplete(OurBiome biomeType)
    {
        yield return null;
        OnBiomeComplete?.Invoke(biomeType);
    }
}