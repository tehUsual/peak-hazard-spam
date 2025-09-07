using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Patches;

[HarmonyPatch]
public static class EruptionSpawnerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EruptionSpawner), "Update")]
    public static bool ReplaceUpdate(EruptionSpawner __instance)
    {
        if (!PhotonNetwork.IsMasterClient) return false;

        float playerZMin = __instance.min.position.z;
        float playerZMax = __instance.max.position.z;
        
        if (!HelperFunctions.AnyPlayerInZRange(playerZMin, playerZMax)) return false;

        __instance.counter -= Time.deltaTime;
        if (__instance.counter < 0f)
        {
            __instance.counter = Random.Range(-3f, 7f);

            Vector3 spawnPos = __instance.transform.position;
            spawnPos.x += Random.Range(-155f, 155f);
            spawnPos.z += Random.Range(-140f, 140f);
            
            __instance.photonView.RPC("RPCA_SpawnEruption", RpcTarget.All, spawnPos);
        }

        return false;
    }
}