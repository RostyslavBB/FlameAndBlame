using FB.Core.Input;
using Zenject;

namespace FB.Core.Installers
{
    /// <summary>
    /// ProjectContext composition root: services that live for the whole app and survive scene restarts.
    /// </summary>
    public sealed class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindInterfacesTo<CursorService>().AsSingle();
        }
    }
}
