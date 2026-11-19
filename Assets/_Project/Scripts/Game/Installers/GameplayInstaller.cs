using FB.Core.Pooling;
using FB.Game.Blame;
using FB.Game.CameraFx;
using FB.Game.Config;
using FB.Game.Effects;
using FB.Game.Fire;
using FB.Game.Flow;
using FB.Game.Hose;
using FB.Game.Interactions;
using FB.Game.Player;
using FB.Game.Signals;
using FB.Game.Surfaces;
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
        [SerializeField] private FireConfig _fireConfig;
        [SerializeField] private InteractionRulesConfig _rulesConfig;

        [Header("Scene")]
        [SerializeField] private PlayerBody _player;
        [SerializeField] private HoseView _hoseView;
        [SerializeField] private Transform _hoseAnchor;
        [SerializeField] private SurfaceCatalog _surfaces;

        [Header("Prefabs")]
        [SerializeField] private WaterSplash _splashPrefab;
        [SerializeField] private FireVfx _fireVfxPrefab;
        [SerializeField] private SteamPuff _steamPrefab;
        [SerializeField] private SparkBurst _sparksPrefab;

        public override void InstallBindings()
        {
            BindConfigs();
            DeclareSignals();
            BindBlame();
            BindPlayer();
            BindHose();
            BindSurfaces();
            BindFire();
            BindFlow();
        }

        private void BindConfigs()
        {
            Container.BindInstance(_palette);
            Container.BindInstance(_playerConfig);
            Container.BindInstance(_roundConfig);
            Container.BindInstance(_hoseConfig);
            Container.BindInstance(_fireConfig);
            Container.BindInstance(_rulesConfig);
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<GamePhaseChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<WaterHitSignal>().OptionalSubscriber();
            Container.DeclareSignal<FireSpreadSignal>().OptionalSubscriber();
            Container.DeclareSignal<FireExtinguishedSignal>().OptionalSubscriber();
            Container.DeclareSignal<ItemRuinedSignal>().OptionalSubscriber();
            Container.DeclareSignal<SurfaceStatusChangedSignal>().OptionalSubscriber();
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
            BindEffect(_splashPrefab, new EffectSpawnSettings(_hoseConfig.MaxActiveSplashes, _hoseConfig.SplashLifetime,
                _hoseConfig.SplashCooldown));
            Container.BindInterfacesTo<WaterJetCollisionHandler>().AsSingle();
        }

        private void BindSurfaces()
        {
            Container.BindInstance(_surfaces);
            Container.BindInterfacesAndSelfTo<SurfaceRegistry>().AsSingle();
            Container.BindInitializableExecutionOrder<SurfaceRegistry>(-100);
            Container.Bind<ISurfaceActions>().To<SurfaceActions>().AsSingle();
            Container.Bind<InteractionRuleRunner>().AsSingle();
            Container.BindInterfacesTo<SurfaceWaterHandler>().AsSingle();
            Container.BindInterfacesTo<SurfaceTintService>().AsSingle();
        }

        private void BindFire()
        {
            Container.BindInterfacesAndSelfTo<FireSimulation>().AsSingle();
            Container.BindInterfacesTo<FireStarter>().AsSingle();
            Container.BindComponentPool(_fireVfxPrefab, _fireConfig.MaxFires);
            Container.BindInterfacesTo<FireVisualService>().AsSingle();
            float lifetime = _fireConfig.OneShotLifetime;
            BindEffect(_steamPrefab, new EffectSpawnSettings(_fireConfig.MaxSteamPuffs, lifetime, 0.1f));
            BindEffect(_sparksPrefab, new EffectSpawnSettings(_fireConfig.MaxSparkBursts, lifetime, 0.1f));
        }

        private void BindEffect<T>(T prefab, EffectSpawnSettings settings) where T : OneShotEffect
        {
            Container.BindComponentPool(prefab, settings.MaxActive);
            Container.BindInterfacesAndSelfTo<OneShotEffectSpawner<T>>().AsSingle().WithArguments(settings);
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
