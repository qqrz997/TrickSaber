using System.Collections;
using TrickSaber.Configuration;
using UnityEngine;
using Zenject;

namespace TrickSaber;

internal class SlowMotionManager
{
    private readonly PluginConfig _config;
    private readonly AudioTimeSyncController _audioTimeSyncController;
    private readonly ICoroutineStarter _coroutineStarter;
    
    private readonly bool _isMultiplayer;
    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();
    
    public SlowMotionManager(
        PluginConfig config,
        AudioTimeSyncController audioTimeSyncController,
        ICoroutineStarter coroutineStarter,
        [InjectOptional] MultiplayerPlayersManager? multiplayerPlayersManager)
    {
        _config = config;
        _audioTimeSyncController = audioTimeSyncController;
        _coroutineStarter = coroutineStarter;
        _isMultiplayer = multiplayerPlayersManager != null;
    }

    private Coroutine? _slowMotionStartAnimation;
    private Coroutine? _slowMotionEndAnimation;
    private float _originalTimeScale;
    private float _targetTimeScale;

    public bool SlowMotionActive { get; private set; }

    public bool CanActivateSlowMotion(TrickAction trickAction)
    {
        return _config.SlowmoDuringThrow
               && trickAction == TrickAction.Throw
               && !_isMultiplayer
               && !SlowMotionActive;
    }

    public void ActivateSlowMotion()
    {
        var timeScale = _audioTimeSyncController.timeScale;
        if (_slowMotionEndAnimation != null)
        {
            _coroutineStarter.StopCoroutine(_slowMotionEndAnimation);
            timeScale = _targetTimeScale;
        }

        _slowMotionStartAnimation = _coroutineStarter.StartCoroutine(SlowMotionStart(_config.SlowmoAmount, timeScale));
        SlowMotionActive = true;
    }

    public void DeactivateSlowMotion()
    {
        if (!SlowMotionActive) return;
        
        if (_slowMotionStartAnimation != null)
        {
            _coroutineStarter.StopCoroutine(_slowMotionStartAnimation);
        }

        _slowMotionEndAnimation = _coroutineStarter.StartCoroutine(SlowMotionEnd());
        SlowMotionActive = false;
    }
    
    private IEnumerator SlowMotionStart(float amount, float originalTimescale)
    {
        var timeScale = _audioTimeSyncController.timeScale;
        _originalTimeScale = originalTimescale;
        var targetTimeScale = _originalTimeScale - amount;
        if (targetTimeScale < 0.1f) targetTimeScale = 0.1f;
        while (timeScale > targetTimeScale)
        {
            timeScale -= _config.SlowmoStepAmount;
            SetTimescale(timeScale);
            yield return _waitForFixedUpdate;
        }

        SetTimescale(targetTimeScale);
    }

    private IEnumerator SlowMotionEnd()
    {
        var timeScale = _audioTimeSyncController.timeScale;
        _targetTimeScale = _originalTimeScale;
        while (timeScale < _targetTimeScale)
        {
            timeScale += _config.SlowmoStepAmount;
            SetTimescale(timeScale);
            yield return _waitForFixedUpdate;
        }
        
        SetTimescale(_targetTimeScale);
    }
    
    private void SetTimescale(float timescale)
    {
        _audioTimeSyncController._timeScale = timescale;
        _audioTimeSyncController._audioSource.pitch = timescale;
    }
}