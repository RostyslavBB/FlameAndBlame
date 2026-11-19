using FB.Core.Pooling;
using FB.Game.Blame;
using FB.Game.CameraFx;
using FB.Game.Config;
using FB.Game.Flow;
using FB.Game.Hose;
using FB.Game.Player;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Installers
{
    /// <summary>SceneContext composition root for the Game scene. All gameplay wiring lives here.</summary>
    public sealed class GameplayInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private PaletteConfig _palette;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private RoundConfig _roundConfig;
        [SerializeField] private HoseConfig _hoseConfig;

        [Header("Scene")]
        [SerializeField] private PlayerBody _player;
        [SerializeField] private HoseView _hoseView;
        [SerializeField] private Transform _hoseAnchor;

        [Header("Prefabs")]
        [SerializeField] private WaterSplash _splashPrefab;

        public override void InstallBindings()
        {
            BindConfigs();
            DeclareSignals();
            BindBlame();
            BindPlayer();
            BindHose();
            BindFlow();
        }

        private void BindConfigs()
        {
            Container.BindInstance(_palette);
            Container.BindInstance(_playerConfig);
            Container.BindInstance(_roundConfig);
            Container.BindInstance(_hoseConfig);
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<GamePhaseChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<WaterHitSignal>().OptionalSubscriber();
        }

        private void BindBlame()
        {
            Container.Bind<ICauseFactory>().To<CauseFactory>().AsSingle();
        }

        private void BindPlayer()
        {
            Container.Bind(typeof(PlayerBody), typeof(IKnockbackReceiver)).FromInstance(_player);
            Container.Bind<IPlayerTraction>().To<PlayerTraction>().AsSingle();
            Container.Bind<PlayerGroundProbe>().AsSingle();
            Container.BindInterfacesTo<PlayerMovement>().AsSingle();
            Container.BindInterfacesTo<PlayerLook>().AsSingle();
            Container.BindInterfacesTo<CameraShaker>().AsSingle();
        }

        private void BindHose()
        {
            Container.BindInstance(_hoseView);
            Container.Bind<Transform>().WithId(HoseIds.Anchor).FromInstance(_hoseAnchor);
            Container.Bind<HosePressure>().AsSingle();
            Container.BindInterfacesTo<HoseController>().AsSingle();
            Container.BindInterfacesTo<HoseRope>().AsSingle();
            Container.BindInterfacesTo<HoseLeash>().AsSingle();
            Container.BindComponentPool(_splashPrefab, _hoseConfig.MaxActiveSplashes);
            Container.BindInterfacesAndSelfTo<SplashSpawner>().AsSingle();
            Container.BindInterfacesTo<WaterJetCollisionHandler>().AsSingle();
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
