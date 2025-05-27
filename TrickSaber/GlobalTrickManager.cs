using TrickSaber.Configuration;
using Zenject;

namespace TrickSaber;

internal class GlobalTrickManager : IInitializable
{
    public SaberTrickManager LeftSaberTrickManager;
    public SaberTrickManager RightSaberTrickManager;
    
    private readonly PluginConfig _config;
    private readonly BeatmapBasicData _beatmapBasicData;
    private readonly SlowMotionManager _slowMotionManager;

    private GlobalTrickManager(PluginConfig config,
        GameplayCoreSceneSetupData gameplayCoreSceneSetup,
        SlowMotionManager slowMotionManager,
        [Inject(Id = SaberType.SaberA)] SaberTrickManager leftTrickManager,
        [Inject(Id = SaberType.SaberB)] SaberTrickManager rightTrickManager) 
    {
        _config = config;
        _slowMotionManager = slowMotionManager;

        _beatmapBasicData = gameplayCoreSceneSetup.beatmapBasicData;

        LeftSaberTrickManager = leftTrickManager;
        RightSaberTrickManager = rightTrickManager;
        _slowMotionManager = slowMotionManager;
    }

    public void Initialize()
    {
        LeftSaberTrickManager.Init(this);
        RightSaberTrickManager.Init(this);
    }

    public void OnTrickStarted(TrickAction trickAction)
    {
        if (_slowMotionManager.CanActivateSlowMotion(trickAction))
        {
            _slowMotionManager.ActivateSlowMotion();
        }
    }

    public void OnTrickEnding(TrickAction trickAction)
    {
        _slowMotionManager.DeactivateSlowMotion();
    }

    public void OnTrickEnded(TrickAction trickAction)
    {
        
    }

    // public bool CanDoTrick() =>
    //     !_config.DisableIfNotesOnScreen 
    //     || _timeSinceLastNote > 20 / _beatmapBasicData.noteJumpMovementSpeed;

    // TODO: investigate unused code
    // public bool IsTrickInState(TrickAction trickAction, TrickState state)
    // {
    //     return LeftSaberTrickManager.IsTrickInState(trickAction, state) ||
    //            RightSaberTrickManager.IsTrickInState(trickAction, state);
    // }
    // public bool IsDoingTrick()
    // {
    //     return LeftSaberTrickManager.IsDoingTrick() || RightSaberTrickManager.IsDoingTrick();
    // }
    // IEnumerator NoteSpawnTimer()
    // {
    //     while (true)
    //     {
    //         _timeSinceLastNote += Time.deltaTime;
    //         yield return null;
    //     }
    // }
    // void OnNoteWasSpawned(NoteController noteController)
    // {
    //     _timeSinceLastNote = 0;
    // }
}