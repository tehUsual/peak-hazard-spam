using System.Collections.Generic;
using UnityEngine;

namespace HazardSpam;

public class TeleportHandler
{
    private Transform? _shoreCampfire;
    private Transform? _tropicsCampfire;
    private Transform? _alpineMesaCampfire;
    
    
    public void Init(List<Biome.BiomeType> currentBiomeTypes)
    {
        foreach (Biome.BiomeType biomeType in currentBiomeTypes)
        {
            switch (biomeType)
            {
                case Biome.BiomeType.Shore:
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
                    
                case Biome.BiomeType.Tropics:
                {
                    var go = GameObject.Find("Map/Biome_2/Jungle/Jungle_Campfire/Campfire");
                    if (go != null)
                    {
                        _tropicsCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Tropics Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Tropics Campfire.");
                    break;
                }

                case Biome.BiomeType.Alpine:
                {
                    var go = GameObject.Find("Map/Biome_3/Snow/Snow_Campfire/Campfire");
                    if (go != null)
                    {
                        _alpineMesaCampfire = go.transform;
                        Plugin.Log.LogInfo($"Found Alpine Campfire");
                    }
                    else
                        Plugin.Log.LogDebug($"Could not find Alpine Campfire.");
                    break;
                }

                case Biome.BiomeType.Mesa:
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
            }
        }
    }

    public void WarpToShoreCampfire()
    {
        if (_shoreCampfire == null) return;
        Player.localPlayer.character.WarpPlayer(_shoreCampfire.position, false);
    }

    public void WarpToTropicsCampfire()
    {
        if (_tropicsCampfire == null) return;
        Player.localPlayer.character.WarpPlayer(_tropicsCampfire.position, false);
    }
    
    public void WarpToAlpineMesaCampfire()
    {
        if (_alpineMesaCampfire == null) return;
        Player.localPlayer.character.WarpPlayer(_alpineMesaCampfire.position, false);
    }
}