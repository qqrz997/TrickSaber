namespace TrickSaber;

internal class SaberControllerBearer
{
    private SaberControllerPackage _left;
    private SaberControllerPackage _right;

    private SaberControllerBearer(SaberManager saberManager, PlayerVRControllersManager playerVrControllersManager)
    {
        _left = new SaberControllerPackage(saberManager.leftSaber, playerVrControllersManager._leftHandVRController);

        _right = new SaberControllerPackage(saberManager.rightSaber, playerVrControllersManager._rightHandVRController);
    }

    public SaberControllerPackage this[SaberType saberType] => saberType == SaberType.SaberA ? _left : _right;

    internal struct SaberControllerPackage
    {
        public Saber Saber { get; }
        public VRController VRController { get; }

        public SaberControllerPackage(Saber saber, VRController vrController)
        {
            Saber = saber;
            VRController = vrController;
        }
    }
}