using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Caldera;

public static class CalderaHandler
{
    private static Transform? _spawner;
    private static PropSpawner? _propSpawner;
    private static GameObject? _prefab;
    private static bool _isSpawnInit = false;
    
    
    public static Transform? GetSpawner => _spawner;
    public static GameObject? GetPrefab => _prefab;
    
    public static void InitCaldera()
    {
        var go = GameObject.Find("Map/Biome_4/Volcano/Caldera_Segment/River");
        if (go == null)
        {
            Plugin.Log.LogError($"[InitCaldera] Could not find LavaRiver in Caldera");
            return;
        }

        var comp = go.AddComponent<LavaRiverSpeedHandler>();
        if (comp == null)
        {
            Plugin.Log.LogError($"[InitCaldera] Could not add LavaRiverSpeedHandler to LavaRiver");
        }
    }


    public static void InitSpawns()
    {
        string sourcePath = "Map/Biome_1/Beach/Beach_Segment/Default/PlateauProps/Jellies";
        string targetPath = "Map/Biome_4/Volcano/Caldera_Segment/Props/Eggs";

        var sourceObj = GameObject.Find(sourcePath);
        var targetObj = GameObject.Find(targetPath);

        if (sourceObj == null)
        {
            Plugin.Log.LogError($"[CalderaHandler.InitSpawns] Could not find {sourcePath}");
            return;
        }

        if (targetObj == null)
        {
            Plugin.Log.LogError($"[CalderaHandler.InitSpawns] Could not find {targetPath}");
            return;
        }

        PropSpawner sourcePs = sourceObj.GetComponent<PropSpawner>();
        PropSpawner targetPs = targetObj.GetComponent<PropSpawner>();

        if (sourcePs == null)
        {
            Plugin.Log.LogError($"[CalderaHandler.InitSpawns] Could not find PropSpawner on {sourcePath}");
            return;
        }
        
        if (targetPs == null)
        {
            Plugin.Log.LogError($"[CalderaHandler.InitSpawns] Could not find PropSpawner on {targetPath}");
            return;
        }
        
        targetPs.props = sourcePs.props;
        targetPs.area.y = 420;

        _spawner = targetObj.transform;
        _propSpawner = targetPs;
        _prefab = targetPs.props[0];
        _isSpawnInit = true;
        
        Plugin.Log.LogInfo($"[CalderaHandler.InitSpawns] Initialized spawns for {sourcePath} -> {targetPath}");
    }

    public static void ClearCaldera()
    {
        var go = GameObject.Find("Map/Biome_4/Volcano/Caldera_Segment/River");
        if (go == null)
        {
            Plugin.Log.LogError($"[ClearCaldera] Could not find LavaRiver in Caldera");
            return;
        }
        
        var comp = go.GetComponent<LavaRiverSpeedHandler>();
        if (comp == null)
        {
            Plugin.Log.LogError($"[ClearCaldera] Could not clear LavaRiverSpeedHandler from LavaRiver");
        }
        else
        {
            comp.enabled = false;
        }
        
        _isSpawnInit = false;
        _spawner = null;
        _propSpawner = null;
        _prefab = null;
    }
    
    public static (Vector3[] positions, Quaternion[] rotations, float[] scaleGains) GenerateSpawnPoints(int attempts)
    {
        if (!PhotonNetwork.IsMasterClient) return ([], [], []);
        if (!_isSpawnInit || _propSpawner == null || _spawner == null || _prefab == null)
        {
            Plugin.Log.LogError("[GenerateSpawnPoints] Could not generate spawn points, not initialized");
            Plugin.Log.LogError(
                $"_isSpawnInit: {_isSpawnInit}, _propSpawner is null: {_propSpawner == null}, _spawner is null: {_spawner == null}, _prefab is null: {_prefab == null}");
            return ([], [], []);
        }
    
        int spawnPosCount = 0;
        int currentAttempt = 0;
        int failedAttempts = 0;

        List<Vector3> positions = [];
        List<Quaternion> rotations = [];
        List<float> scaleGains = [];
        
        while (spawnPosCount < attempts && currentAttempt < attempts * 4)
        {
            var getRandomPoint = _propSpawner.GetType().GetMethod("GetRandomPoint",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (getRandomPoint == null) break;
            
            currentAttempt++;
            try
            {
                var spawnData = getRandomPoint.Invoke(_propSpawner, null);
                if (spawnData == null) continue;

                var pos = (Vector3)spawnData.GetType().GetField("pos").GetValue(spawnData);
                var normal = (Vector3)spawnData.GetType().GetField("normal").GetValue(spawnData);
                var rotation = HelperFunctions.GetRandomRotationWithUp(normal); 

                positions.Add(pos);
                rotations.Add(rotation);
                scaleGains.Add(0f);
                
                spawnPosCount++;
            }
            catch (Exception)
            {
                failedAttempts++;
            }
        }
        Plugin.Log.LogInfo($"Generated {spawnPosCount} spawn points, {failedAttempts} failed (which is fine)");
        
        return (positions.ToArray(), rotations.ToArray(), scaleGains.ToArray());
    }
}