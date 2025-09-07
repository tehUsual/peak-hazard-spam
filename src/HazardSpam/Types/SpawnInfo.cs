using UnityEngine;

namespace HazardSpam.Types;

public class SpawnInfo(Vector3 position, Quaternion rotation, float scaleGain = 0f)
{
    public Vector3 Position = position;
    public Quaternion Rotation = rotation;
    public float ScaleGain = scaleGain;
    //public bool Visible = visible;
}