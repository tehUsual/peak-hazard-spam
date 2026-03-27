using System;
using BepInEx;
using BepInEx.Logging;
using ConsoleTools;
using ConsoleTools.Patches;
using HarmonyLib;
using HazardSpam.Config;
using HazardSpam.Hazards;
using HazardSpam.Menu;
using HazardSpam.Patches;
using NetGameState.Events;
using NetGameState.Logging;
using NetGameState.Patches;
using NetGameState.Tests;
using NetGameState.Util;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam;


[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static Plugin Instance { get; private set; } = null!;
    
    private MenuHandler _menuHandler = null!;

    internal static readonly bool Debug = true;
    internal static readonly bool DebugFull = true;
    internal static readonly bool DebugMenu = false;
    
    internal const int HazardSpamViewID = 9989;
    private const string CompatibleVersion = "1.29.a";

    internal static bool SpawnersInitialized = false;
    
    // PlayerConnectionLog.AddMessage(string s)
    // PlayerConnectionLog.OnPlayerEnteredRoom(player)
    private void Awake()
    {
        Instance = this;
        
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loading..");
        LogProvider.Log = Log;  // enable NetGameState logging
        
        // === Config
        ConfigHandler.Init(Config);
        
        // === Configure console
        if (Debug)
        {
            ConsoleConfig.Register(Name);
            ConsoleConfig.SetLogging(Name, true);
            ConsoleConfig.ShowUnityLogs = false;
            ConsoleConfig.SetDefaultSourceColor(ConsoleColor.DarkCyan);
            ConsoleConfig.SetDefaultCallerColor(ConsoleColor.DarkYellow);    
        }
        
        // === Harmony patch
        var harmony = new Harmony("com.github.tehUsual.HazardSpam");
        
        // HazardSpam patches
        harmony.PatchAll(typeof(SlipperyJellyfishPatches));
        
        // NetGameState patches
        harmony.PatchAll(typeof(AirportCheckInPatches));
        harmony.PatchAll(typeof(LoadScenePatches));
        harmony.PatchAll(typeof(MainMenuPatches));
        harmony.PatchAll(typeof(MapHandlerPatches));
        harmony.PatchAll(typeof(MountainProgressHandlerPatches));
        harmony.PatchAll(typeof(PlayerHandlerPatches));
        harmony.PatchAll(typeof(RunManagerPatches));
        harmony.PatchAll(typeof(SteamLobbyHandlerPatches));
        
        // ConsoleTools patches
        if (Debug)
            harmony.PatchAll(typeof(ConsoleLogListenerPatches));
        
        // === Version check
        if (Application.version.Trim('.') != CompatibleVersion)
            Log.LogColorW($"This plugin has only been tested with v{CompatibleVersion}. The mod may not work correctly."
                                      + $" Current game version: v{Application.version}");

        if (DebugFull)
            CallbackTests.Init();
        
        ModManager.Awake();
        HazardTemplateManager.Initialize();
        
        // === Menu
        _menuHandler = new MenuHandler();
        _menuHandler.Initialize();
        
        Log.LogInfo($"Plugin {Name} load complete!");
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
        if (PhotonNetwork.IsMasterClient)
        {
            // Menu
            if (Input.GetKeyDown(KeyCode.Delete) && GameStateEvents.IsInAirport)
            {
                _menuHandler.Toggle();
            }
            
            // Spawn tests
            //SpawnTests.Update();

            // Utility
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (GameStateEvents.IsRunActive)
                {
                    // Teleports
                    if (Input.GetKeyDown(KeyCode.Keypad1))
                        TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.Shore);
                    if (Input.GetKeyDown(KeyCode.Keypad2))
                        TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.TropicsRoots);
                    if (Input.GetKeyDown(KeyCode.Keypad3))
                        TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.AlpineMesa);
                    if (Input.GetKeyDown(KeyCode.Keypad4))
                        TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.Caldera);
                    if (Input.GetKeyDown(KeyCode.Keypad5))
                        TeleportHandler.TeleportToCampfire(NetGameState.Util.Campfire.PeakFlagpole);
                }

                if (GameStateEvents.IsInAirport)
                {
                    // Quick start
                    if (Input.GetKeyDown(KeyCode.Keypad9))
                    {
                        var go = GameObject.Find("Map/BL_Airport/Fences/Check In desk/AirportGateKiosk");
                        var kiosk = go?.GetComponent<AirportCheckInKiosk>();
                        kiosk?.LoadIslandMaster(0);    
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        ModManager.OnDestroy();
        if (DebugFull)
            CallbackTests.Reset();
    }

    internal static void DestroyGameObject(GameObject go)
    {
        Destroy(go);
    }
    
    public void UpdateSidebarTabCount(string biomeName, int totalHazards)
    {
        _menuHandler.UpdateSidebarTabCount(biomeName, totalHazards);
    }

}
