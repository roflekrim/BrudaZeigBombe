using BrudaZeigBombe.Managers;
using Zenject;

namespace BrudaZeigBombe.Installers;

internal class GameInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CameraManager>().AsSingle();
    }
}