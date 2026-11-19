using FB.Core.Pooling;
using FB.Game.UI;
using UnityEngine;
using Zenject;

namespace FB.Game.Installers
{
    /// <summary>Second SceneContext installer: passive views from the canvas + their presenters (MVP).</summary>
    public sealed class UiInstaller : MonoInstaller
    {
        [SerializeField] private UiConfig _config;
        [SerializeField] private BriefingView _briefing;
        [SerializeField] private HudView _hud;
        [SerializeField] private FeedView _feed;
        [SerializeField] private ReportView _report;
        [SerializeField] private ShockOverlayView _shock;
        [SerializeField] private FeedEntryView _feedEntryPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(_config);
            Container.BindInstance(_briefing);
            Container.BindInstance(_hud);
            Container.BindInstance(_feed);
            Container.BindInstance(_report);
            Container.BindInstance(_shock);
            Container.BindComponentPool(_feedEntryPrefab, _config.FeedMaxEntries + 1);

            Container.BindInterfacesTo<BriefingPresenter>().AsSingle();
            Container.BindInterfacesTo<HudPresenter>().AsSingle();
            Container.BindInterfacesTo<FeedPresenter>().AsSingle();
            Container.Bind<ReportVerdict>().AsSingle();
            Container.BindInterfacesTo<ReportPresenter>().AsSingle();
            Container.BindInterfacesTo<ShockOverlayPresenter>().AsSingle();
        }
    }
}
