using UnityEngine;

namespace HazardSpam.Level.Caldera;

public static class CalderaHandler
{

    public static void InitCaldera()
    {
        var go = GameObject.Find("Map/Biome_4/Volcano/Caldera_Segment/River");
        if (go == null)
        {
            Plugin.Log.LogError($"Could not find LavaRiver in Caldera");
            return;
        }

        var comp = go.AddComponent<LavaRiverSpeedRandomizer>();
        if (comp == null)
        {
            Plugin.Log.LogError($"Could not add LavaRiverSpeedRandomizer to LavaRiver");
        }
    }
}