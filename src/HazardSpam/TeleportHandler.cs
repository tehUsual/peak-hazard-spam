using System.Collections.Generic;
using UnityEngine;

namespace HazardSpam;

public class TeleportHandler
{
    private Transform? _shoreCampfire = null;
    private Transform? _tropicsCampfire = null;
    private Transform? _alpineMesaCampfire = null;
    
    
    public void Init(List<Biome.BiomeType> currentBiomeTypes)
    {
        foreach (Biome.BiomeType biomeType in currentBiomeTypes)
        {
            switch (biomeType)
            {
                case Biome.BiomeType.Shore:
                    _shoreCampfire = GameObject.Find("Map/Biome_1/Beach/Beach_Campfire/Campfire").transform;
                    Plugin.Log.LogInfo($"Found Shore Campfire");
                    break;
                case Biome.BiomeType.Tropics:
                    _tropicsCampfire = GameObject.Find("Map/Biome_2/Jungle/Jungle_Campfire/Campfire").transform;
                    Plugin.Log.LogInfo($"Found Tropics Campfire");
                    break;
                case Biome.BiomeType.Alpine:
                    _alpineMesaCampfire = GameObject.Find("Map/Biome_3/Snow/Snow_Campfire/Campfire").transform;
                    Plugin.Log.LogInfo($"Found Alpine Campfire");
                    break;
                case Biome.BiomeType.Mesa:
                    _alpineMesaCampfire = GameObject.Find("Map/Biome_3/Mesa/Desert_Campfire/Desert_Campfire/Campfire").transform;
                    Plugin.Log.LogInfo($"Found Mesa Campfire");
                    break;
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