using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.LevelStructure;

public static class SpawnerLogic
{
    internal static (Vector3[] positions, Quaternion[] rotations) GenerateSpawnPoints(PropSpawner propSpawner, int attempts)
    {
        int spawnPosCount = 0;
        int currentAttempt = 0;
        int failedAttempts = 0;

        List<Vector3> positions = [];
        List<Quaternion> rotations = [];

        while (spawnPosCount < attempts && currentAttempt < attempts * 2)
        {
            var getRandomPoint = propSpawner.GetType().GetMethod("GetRandomPoint",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (getRandomPoint == null)
                break;
            
            currentAttempt++;
            try
            {
                var spawnData = getRandomPoint.Invoke(propSpawner, null);
                if (spawnData == null)
                    continue;

                var pos = (Vector3)spawnData.GetType().GetField("pos").GetValue(spawnData);
                var normal = (Vector3)spawnData.GetType().GetField("normal").GetValue(spawnData);
                var rotation = HelperFunctions.GetRandomRotationWithUp(normal);

                positions.Add(pos);
                rotations.Add(rotation);
                spawnPosCount++;
            }
            catch (Exception)
            {
                failedAttempts++;
            }
        }
        Plugin.Log.LogInfo($"Generated {spawnPosCount} spawn points. Failed attempts: {failedAttempts}");
        
        return (positions.ToArray(), rotations.ToArray());
    }
}