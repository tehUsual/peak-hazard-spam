using UnityEngine;
using Photon.Pun;

namespace HazardSpam.Level.Caldera;

public class LavaRiverSpeedHandler : MonoBehaviourPun
{
    private Animator _animator = null!;
    private LavaRiverSpeedRandomizer _randomizer = null!;

    private float _targetSpeed;
    private float _applyTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("LavaRiverSpeedHandler: No Animator found!");
            enabled = false;
            return;
        }

        _randomizer = new LavaRiverSpeedRandomizer();
    }

    private void Update()
    {
        // Master polls randomizer
        if (PhotonNetwork.IsMasterClient)
        {
            var (changed, newSpeed) = _randomizer.Poll(Time.deltaTime);
            if (changed)
            {
                // Schedule 2 seconds into the future (network time)
                double applyAt = PhotonNetwork.Time + 2.0;
                photonView.RPC("RPC_SetLavaSpeed", RpcTarget.AllBuffered, newSpeed, applyAt);
            }
        }

        // Apply speed when local Time.time reaches target
        if (_applyTime > 0f && Time.time >= _applyTime)
        {
            _animator.speed = _targetSpeed;
            _applyTime = 0f;
        }
    }

    [PunRPC]
    private void RPC_SetLavaSpeed(float speed, double applyAt)
    {
        _targetSpeed = speed;

        // Convert network time to local Time.time
        _applyTime = (float)(applyAt - PhotonNetwork.Time + Time.time);

        Plugin.Log.LogInfo($"LavaRiverSpeedHandler: New speed {speed} scheduled to apply at {applyAt} network time ({_applyTime} local Time.time)");
    }
}