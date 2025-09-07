using Photon.Pun;
using UnityEngine;

namespace HazardSpam.Level.Caldera;

public class LavaRiverSpeedRandomizer : MonoBehaviour
{
    private readonly float _minSpeed = 1f;
    private readonly float _maxSpeed = 5f;
    private readonly float _randomChangeIntervalMin = 10f;
    private readonly float _randomChangeIntervalMax = 15f;
    private readonly float _spikeIntervalMin = 120f;
    private readonly float _spikeIntervalMax = 150f;
    private readonly float _spikeDuration = 6f;

    private float _timer;
    private float _nextRandomChange;
    private float _nextSpikeTime;
    private float _currentSpeed = 1f;
    private bool _isSpike;
    private float _spikeEndTime;

    private Animator? _animator;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            enabled = false;
            return;
        }
        
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("LavaRiverSpeedRandomizer: No animator found!");
            enabled = false;
        }
    }

    private void Update()
    {
        var (changed, newSpeed) = GetNextSpeed();
        if (changed && _animator != null)
            _animator.speed = newSpeed;
    }


    private (bool, float) GetNextSpeed()
    {
        _timer += Time.deltaTime;

        // Initialize first timings
        if (_nextRandomChange == 0f)
            _nextRandomChange = _timer + Random.Range(_randomChangeIntervalMin, _randomChangeIntervalMax);
        if (_nextSpikeTime == 0f)
            _nextSpikeTime = _timer + Random.Range(_spikeIntervalMin, _spikeIntervalMax);

        // Handle spike
        if (_isSpike)
        {
            if (_timer >= _spikeEndTime)
            {
                _isSpike = false;
                // schedule next random change shortly after spike ends
                _nextRandomChange = _timer + Random.Range(_randomChangeIntervalMin, _randomChangeIntervalMax);
                _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
                return (true, _currentSpeed); // spike ended, speed changed
            }
            return (false, 25f); // spike ongoing, speed is constant, no change
        }

        // Trigger spike
        if (_timer >= _nextSpikeTime)
        {
            _isSpike = true;
            _spikeEndTime = _timer + _spikeDuration;
            _nextSpikeTime = _timer + Random.Range(_spikeIntervalMin, _spikeIntervalMax); // schedule next spike
            return (true, 25f); // new spike, speed changed
        }

        // Handle random speed change (only if not spiking)
        if (_timer >= _nextRandomChange)
        {
            _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
            _nextRandomChange = _timer + Random.Range(_randomChangeIntervalMin, _randomChangeIntervalMax);
            return (true, _currentSpeed); // speed changed
        }

        return (false, _currentSpeed); // no change
    }
}