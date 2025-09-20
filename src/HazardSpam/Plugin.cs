using System;
using System.Collections;
using BepInEx;
using BepInEx.Logging;
using ConsoleTools;
using ConsoleTools.Patches;
using HarmonyLib;
using HazardSpam.Config;
using HazardSpam.LevelStructure;
using HazardSpam.Networking;
using NetGameState.Events;
using NetGameState.Level;
using NetGameState.LevelProgression;
using NetGameState.Util;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam;


[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    
    private const int SpawnerNetworkViewID = 9989;
    
    private const string CompatibleVersion = "1.29.a";

    internal bool SpawnersInitialized = false;
    
    // PlayerConnectionLog.AddMessage(string s)
    // PlayerConnectionLog.OnPlayerEnteredRoom(player)
    private void Awake()
    {
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loaded!");
        
        // === Config
        ConfigHandler.Init(Config);
        
        // === Configure console
        ConsoleConfig.Register(Name);
        ConsoleConfig.SetLogging(Name, true);
        ConsoleConfig.ShowUnityLogs = false;
        ConsoleConfig.SetDefaultSourceColor(ConsoleColor.DarkCyan);
        ConsoleConfig.SetDefaultCallerColor(ConsoleColor.DarkYellow);    
        
        // === Harmony patch
        var harmony = new Harmony("com.github.tehUsual.HazardSpam");
        harmony.PatchAll(typeof(ConsoleLogListenerPatches));
        
        // === Version check
        if (Application.version.Trim('.') != CompatibleVersion)
            Log.LogColorW($"This plugin is only compatible with v{CompatibleVersion}. The library may not work correctly."
                                      + $" Current game version: v{Application.version}");
        
        // === Callbacks
        GameStateEvents.OnRunStartLoading += OnRunStartLoading;
        GameStateEvents.OnRunStartLoadComplete += OnRunStartLoadComplete;
        GameStateEvents.OnAllPlayersReady += OnAllPlayersReady;
        GameStateEvents.OnPlayerLoadTimeout += OnPlayerLoadTimeout;

        SegmentManager.OnSegmentLoading += OnSegmentLoading;
        SegmentManager.OnSegmentLoadComplete += OnSegmentLoadComplete;
        
        GameStateEvents.OnAirportLoaded += OnAirportLoaded;
        GameStateEvents.OnSelfLeaveLobby += OnSelfLeaveLobby;
        
        InitNetwork();
    }

    private void OnRunStartLoading(string sceneName, int ascent)
    {
    }

    private void OnRunStartLoadComplete(string sceneName, int ascent)
    {
        InitializeSpawners();
    }

    private void InitializeSpawners()
    {
        SpawnerNetwork.Instance.StartCoroutine(InitializeSpawnersRoutine());
    }

    private IEnumerator InitializeSpawnersRoutine()
    {
        CleanupSpawners();
        SubZone tropicsZone = SegmentManager.CurrentRunSubZones.TryGetValue(Zone.Tropics, out var value) ? value : SubZone.Unknown;
        SubZone alpineZone = SegmentManager.CurrentRunSubZones.TryGetValue(Zone.Alpine, out value) ? value : SubZone.Unknown;
        
        
        yield return SpawnerRegistry.InitializeSpawnersRoutine((TropicsZone)tropicsZone, (AlpineZone)alpineZone, SegmentManager.IsAlpine);
        yield return SpawnerRegistry.LoadSpawnersRoutine(SegmentManager.CurrentRunSubZones);
        
        SpawnersInitialized = true;
    }

    private void OnAllPlayersReady()
    {
        LoadZone(Zone.Shore);
    }
    
    private void OnPlayerLoadTimeout(Player player)
    {
        Log.LogColorW($"Player {player.photonView.Owner.NickName} did not load");
        LoadZone(SegmentManager.CurrentZone);
    }

    private void OnSegmentLoadComplete(SegmentManager.SegmentInfo segment)
    {
        if (segment.Zone == Zone.Shore)
            return;
        
        LoadZone(segment.Zone);
    }

    private void LoadZone(Zone zone)
    {
        Log.LogColor($"Loading zone {zone}");
        SpawnerNetwork.Instance.StartCoroutine(LoadZoneRoutine(zone));
    }

    private IEnumerator LoadZoneRoutine(Zone zone)
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;
        
        float timeout = 20f;
        float elapsed = 0f;
        
        while (!SpawnersInitialized || !GameStateEvents.IsRunActive)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
            
            if (elapsed >= timeout)
                break;
        }

        if (!SpawnersInitialized)
        {
            Log.LogColorW($"Timed out waiting for spawners to load");
            yield break;
        }
        
        yield return SpawnerRegistry.LoadZoneRoutine(zone);
    }

    private void OnSegmentLoading(SegmentManager.SegmentInfo fromSegment, SegmentManager.SegmentInfo toSegment)
    {
        // TODO: 
        Log.LogColor($"Segment loading: {fromSegment.Zone} -> {toSegment.Zone}");
        
        if (fromSegment.Zone == Zone.Unknown)
            return;
        
        Log.LogColor($"Unloading zone {fromSegment.Zone}");
        SpawnerNetwork.Instance.StartCoroutine(SpawnerRegistry.UnloadZoneRoutine(fromSegment.Zone));
    }

    private void OnAirportLoaded()
    {
        Log.LogColor("Loaded airport, cleaning spawners");
        CleanupSpawners();
    }

    private void OnSelfLeaveLobby()
    {
        Log.LogColor("Left lobby, cleaning spawners");
        CleanupSpawners();
    }

    private void CleanupSpawners()
    {
        SpawnerRegistry.CleanupSpawners();
        SpawnersInitialized = false;
    }

    private void OnDestroy()
    {
        GameStateEvents.OnRunStartLoading -= OnRunStartLoading;
        GameStateEvents.OnRunStartLoadComplete -= OnRunStartLoadComplete;
        GameStateEvents.OnAllPlayersReady -= OnAllPlayersReady;
        GameStateEvents.OnPlayerLoadTimeout -= OnPlayerLoadTimeout;
        
        GameStateEvents.OnAirportLoaded -= OnAirportLoaded;
        GameStateEvents.OnSelfLeaveLobby -= OnSelfLeaveLobby;
    }
    

    private void InitNetwork()
    {
        if (GameObject.Find("HS_SpawnerNetwork") != null)
        {
            Log.LogDebug("HS_SpawnerNetwork already exists.");
            return;
        }

        var go = new GameObject("HS_SpawnerNetwork");
        go.AddComponent<PhotonView>();
        var pv = go.GetComponent<PhotonView>();
        pv.ViewID = SpawnerNetworkViewID;
        go.AddComponent<SpawnerNetwork>();
    }

    private void CleanupNetwork()
    {
        var go = GameObject.Find("HS_SpawnerNetwork");
        if (go != null)
        {
            Destroy(go);
        }
    }

    private void Update()
    {
        // Toggle unity console
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ConsoleConfig.ShowUnityLogs = !ConsoleConfig.ShowUnityLogs;
            Log.LogColor($"Unity console logs {(ConsoleConfig.ShowUnityLogs ? "enabled" : "disabled")}");
        }
        
        // Master only
        if (PhotonNetwork.IsMasterClient && GameStateEvents.IsRunActive)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
                TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.Shore);
            if (Input.GetKeyDown(KeyCode.Keypad2))
                TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.Tropics);
            if (Input.GetKeyDown(KeyCode.Keypad3))
                TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.AlpineMesa);
            if (Input.GetKeyDown(KeyCode.Keypad4))
                TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.Caldera);
            if (Input.GetKeyDown(KeyCode.Keypad5))
                TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.PeakFlagpole);
            
            // Quick start
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                Log.LogColor($"In airport: {GameStateEvents.IsInAirport}");
                if (GameStateEvents.IsInAirport)
                {
                    var go = GameObject.Find("Map/BL_Airport/Fences/Check In desk/AirportGateKiosk");
                    Log.LogColor($"GateKiosk is null: {go == null}");
                    var kiosk = go?.GetComponent<AirportCheckInKiosk>();
                    Log.LogColor($"GateKiosk(script) is null: {kiosk == null}");
                    kiosk?.LoadIslandMaster(0);    
                }
            }
        }
    }
}
