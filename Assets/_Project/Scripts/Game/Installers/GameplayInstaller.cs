using FB.Game.Config;
using FB.Game.Flow;
using FB.Game.Player;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Installers
{
    /// <summary>SceneContext composition root for the Game scene. All gameplay wiring lives here.</summary>
    public sealed class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private PaletteConfig _palette;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private RoundConfig _roundConfig;
        [SerializeField] private PlayerBody _player;

        public override void InstallBindings()
        {
            BindConfigs();
            DeclareSignals();
            BindPlayer();
            BindFlow();
        }

        private void BindConfigs()
        {
            Container.BindInstance(_palette);
            Container.BindInstance(_playerConfig);
            Container.BindInstance(_roundConfig);
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<GamePhaseChangedSignal>().OptionalSubscriber();
        }

        private void BindPlayer()
        {
            Container.Bind(typeof(PlayerBody), typeof(IKnockbackReceiver)).FromInstance(_player);
            Container.Bind<IPlayerTraction>().To<PlayerTraction>().AsSingle();
            Container.Bind<PlayerGroundProbe>().AsSingle();
            Container.BindInterfacesTo<PlayerMovement>().AsSingle();
            Container.BindInterfacesTo<PlayerLook>().AsSingle();
        }

        private void BindFlow()
        {
            Container.BindInterfacesAndSelfTo<RoundClock>().AsSingle();
            Container.Bind<IGameState>().To<BriefingState>().AsSingle();
            Container.Bind<IGameState>().To<PlayingState>().AsSingle();
            Container.Bind<IGameState>().To<ReportState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesTo<RestartController>().AsSingle();
        }
    }
}
