using System.Collections.Generic;
using HazardSpam.Level;
using Photon.Pun;
using UnityEngine;

namespace HazardSpam;

public class TeleportHandler
{
    private Transform? _shoreCampfire;
    private Transform? _tropicsCampfire;
    private Transform? _alpineMesaCampfire;
    private Transform? _calderaCampfire;


    public void Init(List<OurBiome> currentBiomeTypes)
    {
        foreach (OurBiome biomeType in currentBiomeTypes)
        {
            switch (biomeType)
            {
                case OurBiome.Shore:
                {
                    var go = GameObject.Find("Map/Biome_1/Beach/Beach_Campfire/Campfire");
                    if (go != null)
                    {
                        _shoreCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Shore Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Shore Campfire.");
                    break;
                }

                case OurBiome.Tropics:
                {
                    var go = GameObject.Find("Map/Biome_2/Tropics/Jungle_Campfire/Campfire");
                    if (go != null)
                    {
                        _tropicsCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Tropics Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Tropics Campfire.");
                    break;
                }

                case OurBiome.Alpine:
                {
                    var go = GameObject.Find("Map/Biome_3/Alpine/Snow_Campfire/Campfire");
                    if (go != null)
                    {
                        _alpineMesaCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Alpine Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Alpine Campfire.");
                    break;
                }

                case OurBiome.Mesa:
                {
                    var go = GameObject.Find("Map/Biome_3/Mesa/Desert_Campfire/Snow_Campfire/Campfire");
                    if (go != null)
                    {
                        _alpineMesaCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Mesa Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Mesa Campfire.");
                    break;
                }

                case OurBiome.Caldera:
                {
                    var go = GameObject.Find("Map/Biome_4/Volcano/Volcano_Campfire/Campfire");
                    if (go != null)
                    {
                        _calderaCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Volcano Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Volcano Campfire.");
                    break;
                }
            }
        }
    }

    public void WarpToShoreCampfire()
    {
        if (_shoreCampfire == null)
            return;

        foreach (var character in PlayerHandler.GetAllPlayerCharacters())
        {
            character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, _shoreCampfire.position, false);
        }
    }

    public void WarpToTropicsCampfire()
    {
        if (_tropicsCampfire == null)
            return;

        foreach (var character in PlayerHandler.GetAllPlayerCharacters())
        {
            character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, _tropicsCampfire.position, false);
        }
    }

    public void WarpToAlpineMesaCampfire()
    {
        if (_alpineMesaCampfire == null)
            return;

        foreach (var character in PlayerHandler.GetAllPlayerCharacters())
        {
            character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, _alpineMesaCampfire.position, false);
        }
    }

    public void WarpToCalderaCampfire()
    {
        if (_calderaCampfire == null)
            return;

        foreach (var character in PlayerHandler.GetAllPlayerCharacters())
        {
            character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, _calderaCampfire.position, false);
        }
    }
}