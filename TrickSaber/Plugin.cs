using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Zenject;
using TrickSaber.Configuration;
using TrickSaber.Installers;

namespace TrickSaber;

[Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
public class Plugin
{
    public static Logger Log { get; private set; } = null!;
    
    [Init]
    public Plugin(Logger logger, Config config, Zenjector zenjector, PluginMetadata pluginMetadata)
    {
        Log = logger;

        zenjector.UseLogger(logger);
        zenjector.UseHttpService();
        zenjector.Install<AppInstaller>(Location.App, config.Generated<PluginConfig>());
        zenjector.Install<MenuInstaller>(Location.Menu);
        zenjector.Install<GameInstaller>(Location.StandardPlayer);
        
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} has been initialized.");
    }
}