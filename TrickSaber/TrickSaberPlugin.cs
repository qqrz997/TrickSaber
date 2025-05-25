using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Hive.Versioning;
using Newtonsoft.Json;
using SiraUtil;
using SiraUtil.Logging;
using SiraUtil.Tools;
using SiraUtil.Web;
using UnityEngine.XR;
using Zenject;
using Version = Hive.Versioning.Version;

namespace TrickSaber;

class TrickSaberPlugin : IInitializable
{
    public bool Initialized;

    public string ControllerModel;
    public bool IsKnucklesController => ControllerModel.Contains("Knuckles");

    public Version Version;
    public Version RemoteVersion;
    public bool IsNewestVersion = true;

    private readonly SiraLog _logger;

    public TrickSaberPlugin(SiraLog logger)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        var ver = Assembly.GetExecutingAssembly().GetName().Version;
        Version = new Version(ver.Major, ver.Minor, ver.Build);

        ControllerModel = GetControllerName();
        Initialized = true;

        _logger.Debug($"TrickSaber version {Version} started");
    }

    public string GetControllerName()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!device.isValid) device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!device.isValid) return "";
        return device.name;
    }

    internal class Release
    {
        [JsonProperty("tag_name")] public string TagName;
    }
}