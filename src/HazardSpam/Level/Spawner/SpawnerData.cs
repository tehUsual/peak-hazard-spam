using System;
using System.Collections.Generic;
using HazardSpam.Spawning;
using HazardSpam.Types;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Level.Spawner;

public class SpawnerData
{
    public PropSpawner PropSpawner { get; }
    public GameObject Prefab { get; }
    private int _defaultNrOfSpawns;
    private readonly List<SpawnInfo> _spawnInfo = [];
    
    public Biome.BiomeType BiomeType { get; private set; }
    public BiomeArea Area { get; private set; }
    public SpawnType SpawnType { get; private set; }
    
    
    public SpawnerData(PropSpawner propSpawner, GameObject prefab, Biome.BiomeType biomeType, BiomeArea area, SpawnType spawnType)
    {
        PropSpawner = propSpawner;
        Prefab = prefab;
        _defaultNrOfSpawns = PropSpawner.nrOfSpawns;
        BiomeType = biomeType;
        Area = area;
        SpawnType = spawnType;
    }

    public void Clear()
    {
        _spawnInfo.Clear();
    }

    public void AddSpawnInfos(Vector3[] positions, Quaternion[] rotations, float[]? scaleGains)
    {
        if (positions.Length == _spawnInfo.Count) return;
        
        for (int i = 0; i < positions.Length; i++)
        {
            if (scaleGains != null)
                _spawnInfo.Add(new SpawnInfo(positions[i], rotations[i], scaleGains[i]));
            else
                _spawnInfo.Add(new SpawnInfo(positions[i], rotations[i]));
        }
    }

    public void AddSpawnInfo(Vector3 position, Quaternion rotation, float? scaleGain = null)
    {
        if (scaleGain != null)
            _spawnInfo.Add(new SpawnInfo(position, rotation, scaleGain.Value));
        else
            _spawnInfo.Add(new SpawnInfo(position, rotation));
    }

    public (Vector3[] positions, Quaternion[] rotations, float[] scaleGains) GetSpawnInfoArrays()
    {
        int count = _spawnInfo.Count;
        Vector3[] positions = new Vector3[count];
        Quaternion[] rotations = new Quaternion[count];
        float[] scaleGains = new float[count];
        
        for (int i = 0; i < count; i++)
        {
            positions[i] = _spawnInfo[i].Position;
            rotations[i] = _spawnInfo[i].Rotation;
            scaleGains[i] = _spawnInfo[i].ScaleGain;
        }
        return (positions, rotations, scaleGains);
    }

    public (Vector3[] positions, Quaternion[] rotations, float[] scaleGains) GenerateSpawnPoints(int attempts)
    {
        if (!PhotonNetwork.IsMasterClient) return ([], [], []);
        
        int spawnPosCount = 0;
        int currentAttempt = 0;
        int failedAttempts = 0;

        List<Vector3> positions = [];
        List<Quaternion> rotations = [];
        List<float> scaleGains = [];
        
        while (spawnPosCount < attempts && currentAttempt < attempts)
        {
            var getRandomPoint = PropSpawner.GetType().GetMethod("GetRandomPoint",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (getRandomPoint == null) break;
            
            currentAttempt++;
            try
            {
                var spawnData = getRandomPoint.Invoke(PropSpawner, null);
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
        Plugin.Log.LogInfo($"Generated {spawnPosCount} spawn points, {failedAttempts} (which is fine)");
        
        return (positions.ToArray(), rotations.ToArray(), scaleGains.ToArray());
    }
}