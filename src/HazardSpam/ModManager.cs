using System;
using System.Collections;
using ConsoleTools;
using HazardSpam.Hazards;
using HazardSpam.Menu.Descriptors;
using HazardSpam.Menu.Settings;
using HazardSpam.Networking;
using HazardSpam.Tests;
using HazardSpam.Types;
using NetGameState.Events;
using NetGameState.Level;
using NetGameState.LevelProgression;
using NetGameState.Listeners;
using NetGameState.Network;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam;

public static class ModManager
{
    internal static void Awake()
    {
        InitNetworkObjects();

        // === Callbacks
        GameStateEvents.OnRunStartLoading += OnRunStartLoading;
        GameStateEvents.OnRunStartLoadComplete += OnRunStartLoadComplete;

        GameStateEvents.OnAllPlayersReady += OnAllPlayersReady;
        GameStateEvents.OnRunStartedAndPlayersReady += OnRunStartedAndPlayersReady;

        SegmentManager.OnSegmentLoading += OnSegmentLoading;
        SegmentManager.OnSegmentLoadComplete += OnSegmentLoadComplete;

        GameStateEvents.OnAirportLoaded += OnAirportLoaded;
        GameStateEvents.OnSelfLeaveLobby += OnSelfLeaveLobby;
    }

    internal static void OnDestroy()
    {
        GameStateEvents.OnRunStartLoading -= OnRunStartLoading;
        GameStateEvents.OnRunStartLoadComplete -= OnRunStartLoadComplete;

        GameStateEvents.OnAllPlayersReady -= OnAllPlayersReady;
        GameStateEvents.OnRunStartedAndPlayersReady -= OnRunStartedAndPlayersReady;

        SegmentManager.OnSegmentLoading -= OnSegmentLoading;
        SegmentManager.OnSegmentLoadComplete -= OnSegmentLoadComplete;

        GameStateEvents.OnAirportLoaded -= OnAirportLoaded;
        GameStateEvents.OnSelfLeaveLobby -= OnSelfLeaveLobby;

        CleanupSpawners();
        CleanupNetwork();
    }

    private static void InitNetworkObjects()
    {
        if (!ReferenceEquals(GameObject.Find("HazardSpamNetwork"), null))
        {
            Plugin.Log.LogDebug("HazardSpamNetwork already exists.");
            return;
        }

        var go = new GameObject("HazardSpamNetwork");
        go.AddComponent<PlayerReadyTracker>();
        go.AddComponent<PhotonCallbacks>();
        go.AddComponent<PhotonView>();

        var pv = go.GetComponent<PhotonView>();
        pv.ViewID = Plugin.HazardSpamViewID;

        go.AddComponent<NetComm>();
    }

    private static void CleanupNetwork()
    {
        var go = GameObject.Find("HazardSpamNetwork");
        if (!ReferenceEquals(go, null))
        {
            // TODO: free the allocated view ID manually ??
            Plugin.DestroyGameObject(go);
        }
    }

    private static void CleanupSpawners()
    {
        HazardTemplateManager.Reset();
        HazardManager.Reset();
        Plugin.SpawnersInitialized = false;
    }

    // ============================================================================
    //  Callbacks - Lobby
    // ============================================================================
    private static void OnAirportLoaded()
    {
        Plugin.Log.LogColor("Loaded airport, cleaning spawners");
        CleanupSpawners();
    }

    private static void OnSelfLeaveLobby()
    {
        Plugin.Log.LogColor("Left lobby, cleaning spawners");
        CleanupSpawners();
    }


    // ============================================================================
    //  Callbacks - Run start
    // ============================================================================
    private static void OnRunStartLoading(string sceneName, int ascent)
    {
    }

    private static void OnAllPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        //NetComm.Instance.StartCoroutine(SpawnTests.InitializeRunTypeTest());
        //NetComm.Instance.StartCoroutine(SpawnTests.InitializeRunSegmentTest());
    }

    private static void OnRunStartLoadComplete(string sceneName, int ascent)
    {
        // Everyone
        HazardTemplateManager.LoadSceneData();
    }

    private static void OnRunStartedAndPlayersReady()
    {
        Plugin.Log.LogColor("Run started and all players are ready");
        // Master only
        if (!PhotonNetwork.IsMasterClient)
            return;

        NetComm.Instance.StartCoroutine(LoadRun());
    }

    private static IEnumerator LoadRun()
    {
        Plugin.Log.LogColor("Waiting to load run.. 3 seconds");
        yield return new WaitForSeconds(3f);            // ensure all players have loaded their spawner prefabs

        Plugin.Log.LogColor("Creating spawners..");
        HazardManager.CreateSpawnersFromConfigOverNetwork();
        Plugin.Log.LogColor("Spawners created");

        yield return new WaitForSeconds(0.5f);
        
        Plugin.Log.LogColor("Applying hazard tweaks..");
        HazardManager.ApplyTweaksFromConfigOverNetwork();
        Plugin.Log.LogColor("Hazard tweaks applied");
        
        yield return new WaitForSeconds(3f);
        
        Plugin.Log.LogColor("Loading zone");
        yield return HazardManager.LoadZone(Zone.Shore);
    }


// ============================================================================
    //  Callbacks - Segments
    // ============================================================================
    private static void OnSegmentLoading(SegmentManager.SegmentInfo fromSegment, SegmentManager.SegmentInfo toSegment)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        Plugin.Log.LogColor($"FROM: {fromSegment.Chapter.ToString()}/{fromSegment.Zone.ToString()}/{fromSegment.SubZone.ToString()} - TO: {toSegment.Chapter.ToString()}/{toSegment.Zone.ToString()}/{toSegment.SubZone.ToString()}");
        Plugin.Log.LogColor($"Segment loading: {fromSegment.Zone} -> {toSegment.Zone}");
        
        if (fromSegment.Zone == Zone.Unknown)
            return;
        
        HazardManager.UnloadZone(fromSegment.Zone);
    }
    
    private static void OnSegmentLoadComplete(SegmentManager.SegmentInfo segment)
    {
        Plugin.Log.LogColor($"LOADED: {segment.Chapter.ToString()}/{segment.Zone.ToString()}/{segment.SubZone.ToString()}");
        
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (segment.Zone == Zone.Shore)
            return;

        NetComm.Instance.StartCoroutine(HazardManager.LoadZone(segment.Zone));
        //NetComm.Instance.StartCoroutine(SpawnTests.LoadZone(segment.Zone));
    }
}