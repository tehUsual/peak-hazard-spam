using System.Collections;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HazardSpam.Level;
using HazardSpam.Networking;
using HazardSpam.Patches;
using HazardSpam.Util;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HazardSpam;


[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    
    private const int SpawnerNetworkViewID = 9989;

    private bool _isGameStarted;

    public readonly bool Debug = true;

    private readonly TeleportHandler _teleportHandler = new TeleportHandler();
    
    // PlayerConnectionLog.AddMessage(string s)
    // PlayerConnectionLog.OnPlayerEnteredRoom(player)
    private void Awake()
    {
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loaded!");
        
        // Harmony patch
        var harmony = new Harmony("com.github.tehUsual.HazardSpam");
        harmony.PatchAll(typeof(LevelChangePatches));
        harmony.PatchAll(typeof(EruptionSpawnerPatches));
        
        // network helper
        //InitNetwork();
        
        // Callbacks
        SceneManager.sceneLoaded += OnSceneLoaded;
        LevelState.OnBiomeLoading += OnBiomeLoading;
        LevelState.OnBiomeComplete += OnBiomeComplete;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    

    private void InitNetwork()
    {
        if (GameObject.Find("HS_SpawnerNetwork") != null)
        {
            Log.LogDebug("HS_SpawnerNetwork already exists.");
            return;
        }

        var go = new GameObject("HS_SpawnerNetwork");
        DontDestroyOnLoad(go);
        go.AddComponent<PhotonView>();
        var pv = go.GetComponent<PhotonView>();
        pv.ViewID = SpawnerNetworkViewID;
        go.AddComponent<SpawnerNetwork>();
    }

    private IEnumerator DelayedInitNetwork()
    {
        yield return new WaitForSeconds(0.5f);
        InitNetwork();
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
        if (PhotonNetwork.IsMasterClient)
        {
            if (_isGameStarted && Debug)
            {
                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    Player.localPlayer.character.refs.balloons.TieNewBalloon(0);
                }

                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    if (Player.localPlayer.character.refs.balloons.tiedBalloons.Count > 0)
                    {
                        var balloon = Player.localPlayer.character.refs.balloons.tiedBalloons[0];
                        Player.localPlayer.character.refs.balloons.RemoveBalloon(balloon);
                    }
                }
                
                // Teleports
                if (Input.GetKeyDown(KeyCode.Keypad1))
                    _teleportHandler.WarpToShoreCampfire();
                if (Input.GetKeyDown(KeyCode.Keypad2))
                    _teleportHandler.WarpToTropicsCampfire();
                if (Input.GetKeyDown(KeyCode.Keypad3))
                    _teleportHandler.WarpToAlpineMesaCampfire();
            }
        }
    }

    private void OnBiomeLoading(Biome.BiomeType biomeType)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Log.LogInfo($"[Main] Biome loading: {biomeType}");

        switch (biomeType)
        {
            case Biome.BiomeType.Tropics:
                LevelManager.ClearBiome(Biome.BiomeType.Shore);
                break;
            case Biome.BiomeType.Alpine:
            case Biome.BiomeType.Mesa:
                LevelManager.ClearBiome(Biome.BiomeType.Tropics);
                break;
            case Biome.BiomeType.Volcano:
                LevelManager.ClearBiome(Biome.BiomeType.Alpine);
                LevelManager.ClearBiome(Biome.BiomeType.Mesa);
                break;
        }
    }
    
    private void OnBiomeComplete(Biome.BiomeType biomeType)
    {
        // Make sure everyone initializes the level manager
        if (biomeType == Biome.BiomeType.Shore)
            LevelManager.Init(LevelState.GetBiomeTypes());
        
        
        if (!PhotonNetwork.IsMasterClient) return;
        
        Log.LogInfo($"Biome complete: {biomeType}");

        switch (biomeType)
        {
            case Biome.BiomeType.Shore:
                _teleportHandler.Init(LevelState.GetBiomeTypes());
                LevelManager.InitBiome(Biome.BiomeType.Shore);
                break;
            case Biome.BiomeType.Tropics:
                LevelManager.InitBiome(Biome.BiomeType.Tropics);
                break;
            case Biome.BiomeType.Alpine:
                LevelManager.InitBiome(Biome.BiomeType.Alpine);
                break;
            case Biome.BiomeType.Mesa:
                LevelManager.InitBiome(Biome.BiomeType.Mesa);
                break;
            case Biome.BiomeType.Volcano:
                LevelManager.InitBiome(Biome.BiomeType.Volcano);
                break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Log.LogInfo($"Scene loaded: {scene.name}");
        
        // Game start detection grabbed from https://github.com/PEAKModding/PEAKLib/blob/main/tests/PEAKLib.Tests/Plugin.cs
        Match match = new Regex(@"^Level_(\d+)$").Match(scene.name);
        if (mode == LoadSceneMode.Single && match.Success &&
            int.TryParse(match.Groups[1].Value, out _) &&
            PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Log.LogInfo("Game Start detected");
            _isGameStarted = true;
        }

        if (scene.name == "Airport")
        {
            StatusMessageHandler.DeInit();
            CleanupNetwork();
            StartCoroutine(DelayedInitNetwork());
            LevelManager.Reset();
        }
    }
}
