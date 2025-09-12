using UnityEngine;

namespace HazardSpam.Level.Caldera;

public class LavaRiverSpeedRandomizer
{
    private float _timer;
    private float _nextRandomChange;
    private float _nextSpikeTime;

    private readonly float _randomChangeInterval = 10f;
    private readonly float _spikeInterval = 120f;
    private readonly float _spikeDurationMin = 8f;
    private readonly float _spikeDurationMax = 12f;
    private readonly float _minSpeed = 1f;
    private readonly float _maxSpeed = 5f;
    private readonly float _minSpikeSpeed = 25f;
    private readonly float _maxSpikeSpeed = 30f;

    public LavaRiverSpeedRandomizer()
    {
        _timer = 0f;
        _nextRandomChange = _randomChangeInterval;
        _nextSpikeTime = _spikeInterval;
    }
    
    public (bool, float) Poll(float deltaTime)
    {
        _timer += deltaTime;

        // Spike check
        if (_timer >= _nextSpikeTime)
        {
            float spikeTime = Random.Range(_spikeDurationMin, _spikeDurationMax);
            float newSpeed = Random.Range(_minSpikeSpeed, _maxSpikeSpeed);
            
            // schedule next spike and random change
            _nextSpikeTime += _spikeInterval + spikeTime;
            _nextRandomChange = _timer + spikeTime;
            
            return (true, newSpeed);
        }

        // Regular random speed
        if (_timer >= _nextRandomChange)
        {
            _nextRandomChange += _randomChangeInterval;
            float newSpeed = Random.Range(_minSpeed, _maxSpeed);
            return (true, newSpeed);
        }

        return (false, 0f);
    }
}