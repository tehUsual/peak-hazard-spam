using ConsoleTools;
using HazardSpam.LevelStructure;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Networking;

public class SpawnerNetwork : MonoBehaviourPun
{
    public static SpawnerNetwork Instance { get; private set; } = null!;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void SpawnPropsNetworkNew(string id, Vector3[] positions, Quaternion[] rotations)
    {
        if (positions.Length == 0)
            return;
        
        Plugin.Log.LogColor($"[Network] Sending 'RPC_SpawnProps' for {positions.Length} at {id}");
        
        photonView.RPC("RPC_SpawnPropsNew", RpcTarget.All, id, positions, rotations);
    }

    [PunRPC]
    public void RPC_SpawnPropsNew(string id, Vector3[] positions, Quaternion[] rotations)
    {
        Plugin.Log.LogColor($"[Network] Received 'RPC_SpawnProps' for {positions.Length} at {id}");
        SpawnerRegistry.InstantiateMultipleById(id, positions, rotations);
    }
    
    internal GameObject? InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(prefab, position, rotation, parent);
    }

    public void ClearPropsNetworkNew(string id)
    {
        Plugin.Log.LogColor($"[Network] Sending 'RPC_ClearPropsNew' {id}");

        photonView.RPC("RPC_ClearPropsNew", RpcTarget.All, id);
    }

    [PunRPC]
    public void RPC_ClearPropsNew(string id)
    {
        Plugin.Log.LogColor($"[Network] Received 'RPC_ClearPropsNew' {id}");
        SpawnerRegistry.DestroySpawnsById(id);
    }

    internal void DestroyGameObject(GameObject go)
    {
        Destroy(go);
    }






    private void Trash()
    {
        /*public void SpawnPropsNetworkCaldera(Vector3[] positions, Quaternion[] rotations, float[] scaleGains)
        {
            if (positions.Length == 0) return;
            Plugin.Log.LogInfo($"[Network] Sending 'RPC_SpawnProps' for {positions.Length} at {nameof(Biome.BiomeType.Volcano)}/Plateau/SlipperyJellyfish");

            photonView.RPC(
                "RPC_SpawnProps",
                RpcTarget.All,
                Biome.BiomeType.Volcano, BiomeArea.Plateau, SpawnType.Jellies,
                positions, rotations
            );
        }*/

        // private IEnumerator SpawnPropsInWorldCaldera(Vector3[] positions, Quaternion[] rotations)
        // {
        //     Transform? spawnerTrans = CalderaHandler.GetSpawner;
        //     GameObject? prefab = CalderaHandler.GetPrefab;
        //
        //     if (spawnerTrans == null || prefab == null)
        //     {
        //         Plugin.Log.LogError("Could not find Caldera spawner or prefab");
        //         yield break;    
        //     }
        //     
        //     int spawns = 0;
        //     for (int i = 0; i < positions.Length; i++)
        //     {
        //         var go = Instantiate(prefab, positions[i], rotations[i], spawnerTrans);
        //         if (go != null)
        //         {
        //             go.AddComponent<SpawnedPropHS>();
        //             spawns++;
        //         }
        //         
        //         if (i % 50 == 0 && i != 0) yield return null;
        //     }
        //     Plugin.Log.LogInfo($"Spawned {spawns}/{positions.Length} '{prefab.name}' in Caldera/Plateau");
        // }
    }
}