using HarmonyLib;
using Photon.Pun;
using UnityEngine.SceneManagement;
using HazardSpam.Level;

namespace HazardSpam.Patches;

[HarmonyPatch]
public static class LevelChangePatches
{
    public static bool PatchSuccessfull { get; private set; }
    public static AirportCheckInKiosk AirportCheckInKioskInstance { get; private set; } = null!;
    

    // Get an instance of the Maphandler
    [HarmonyPatch(typeof(MapHandler), "Awake")]
    [HarmonyPostfix]
    public static void PostfixAwake(MapHandler __instance)
    {
        LevelState.Initialize(__instance);
        PatchSuccessfull = true;
        Plugin.Log.LogInfo("[MapHandler.Awake] MapHandler fetched!");
    }

    // Detect beach loading
    [HarmonyPatch(typeof(AirportCheckInKiosk), "BeginIslandLoadRPC")]
    [HarmonyPrefix]
    public static void PrefixBeginIslandLoadRPC(AirportCheckInKiosk __instance)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        Plugin.Log.LogInfo("[AirportCheckInKiosk.BeginIslandLoadRPC] LOADING SHORE");
        AirportCheckInKioskInstance = __instance;
        LevelState.SetBiomeComplete(Biome.BiomeType.Shore);
    }

    // Detect beach loaded
    [HarmonyPatch(typeof(RunManager), "StartRun")]
    [HarmonyPostfix]
    public static void PostfixStart(RunManager __instance)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (SceneManager.GetActiveScene().name == "Airport") return;

        if (SceneManager.GetActiveScene().name.StartsWith("Level_"))
        {
            Plugin.Log.LogInfo("[RunManager.Start] LOADED SHORE");
            LevelState.SetBiomeComplete(Biome.BiomeType.Shore);    
        }
        else
        {
            SceneManager.sceneLoaded += OnLevelSceneLoaded;
        }
    }
    
    #pragma warning disable Harmony003
    private static void OnLevelSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.StartsWith("Level_")) return;
        
        Plugin.Log.LogInfo("[RunManager.Start] LOADED SHORE");
        LevelState.SetBiomeComplete(Biome.BiomeType.Shore);
        SceneManager.sceneLoaded -= OnLevelSceneLoaded;
    }
    #pragma warning restore Harmony003

    // Detect segment loading
    [HarmonyPatch(typeof(MapHandler), "GoToSegment")]
    [HarmonyPostfix]
    public static void PostfixGoToSegment(Segment s)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Plugin.Log.LogInfo($"[MapHandler.GoToSegment] LOADING BIOME: {s}");
        LevelState.SetBiomeLoading();
    }

    // Detect segment loaded
    [HarmonyPatch(typeof(MountainProgressHandler), "SetSegmentComplete")]
    [HarmonyPostfix]
    public static void PostfixSetSegmentComplete(int segment)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Plugin.Log.LogInfo($"[MountainProgressHandler.SetSegmentComplete] LOADED BIOME: {segment}");
        LevelState.SetBiomeComplete();
    }
}