using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using BrudaZeigBombe.Configuration;
using BrudaZeigBombe.Installers;
using IPALogger = IPA.Logging.Logger;

namespace BrudaZeigBombe;

[Plugin(RuntimeOptions.SingleStartInit)]
public class Plugin
{
    private PluginConfig? _config;
    
    [Init]
    public void Init(Zenjector zenjector, IPALogger logger, Config config)
    {
        _config = config.Generated<PluginConfig>();
        
        zenjector.UseLogger(logger);
        zenjector.UseMetadataBinder<Plugin>();

        zenjector.Install<AppInstaller>(Location.App, _config);
        zenjector.Install<GameInstaller>(Location.GameCore);
    }

    [OnEnable]
    public void OnEnable()
    {
        BSMLSettings.instance.AddSettingsMenu("BrudaZeigBombe", "BrudaZeigBombe.UI.settings.bsml", _config);
    }

    [OnDisable]
    public void OnDisable() { }
}