using System.Reflection;
using BrudaZeigBombe.Affinity_Patches;
using BrudaZeigBombe.Configuration;
using UnityEngine;
using Zenject;

namespace BrudaZeigBombe.Installers;

internal class AppInstaller : Installer
{
    private static AssetBundle? AssetBundle;
    
    private readonly PluginConfig _config;

    public AppInstaller(PluginConfig config)
    {
        _config = config;
    }

    public override void InstallBindings()
    {
        Container.BindInstance(_config);

        if (AssetBundle is null)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BrudaZeigBombe.Resources.Shader");
            AssetBundle = AssetBundle.LoadFromStream(stream);
        }

        var shader = AssetBundle.LoadAsset<Shader>("assets/shaders/shaderpack/rk_unlit transparent stripped.shader");
        Container.BindInstance(new Material(shader)).WithId("BZB_Material");

        Container.BindInterfacesAndSelfTo<BombPatch>().AsSingle();
    }
}