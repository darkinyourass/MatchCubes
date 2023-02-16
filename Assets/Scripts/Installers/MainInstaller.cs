using Common.Gateway;
using Common.Presenter;
using Common.Usecase;
using Common.View;
using Cubes;
using DefaultNamespace;
using UI;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private TouchMovement _touchMovement;
    [SerializeField] private TestGrid _testGrid;
    [SerializeField] private Timer _timer;
    [SerializeField] private UIStateMachine _uiStateMachine;
    [SerializeField] private GetCoinsButton _getCoinsButton;
    [SerializeField] private CoinsHolder _coinsHolder;
    [SerializeField] private WinCondition _winCondition;

    public override void InstallBindings()
    {
        // Gateways
        var colorGateway = new ColorGateway();
        var progressBarGateway = new ProgressBarGateway();
        
        // Color type
        var colorUsecase = new ColorUsecase(colorGateway);
        var colorPresenter = gameObject.AddComponent<ColorPresenter>();
        colorPresenter.Initialize(colorUsecase);
        Container.Bind<IColorGateway>().FromInstance(colorGateway).AsTransient().NonLazy();
        Container.Bind<IColorUsecase>().FromInstance(colorUsecase).AsTransient().NonLazy();
        Container.Bind<IColorPresenter>().FromInstance(colorPresenter).AsTransient().NonLazy();
        
        //ProgressBar
        var progressBarUsecase = new ProgressBarUsecase(progressBarGateway);
        var progressBarPresenter = gameObject.AddComponent<ProgressBarPresenter>();
        progressBarPresenter.Initialize(progressBarUsecase);
        
        Container.Bind<IProgressBarGateway>().FromInstance(progressBarGateway).AsTransient().NonLazy();
        Container.Bind<IProgressBarUsecase>().FromInstance(progressBarUsecase).AsTransient().NonLazy();
        Container.Bind<IProgressBarPresenter>().FromInstance(progressBarPresenter).AsTransient().NonLazy();
        
        BindTouchMovement();
        BindGrid();
        BindTimer();
        BindUI();
    }

    private void BindTouchMovement()
    {
        Container.Bind<TouchMovement>().FromInstance(_touchMovement).AsSingle().NonLazy();
    }

    private void BindGrid()
    {
        Container.Bind<TestGrid>().FromInstance(_testGrid).AsSingle().NonLazy();
    }

    private void BindTimer()
    {
        Container.Bind<Timer>().FromInstance(_timer).AsSingle().NonLazy();
    }

    private void BindUI()
    {
        Container.Bind<UIStateMachine>().FromInstance(_uiStateMachine).AsSingle().NonLazy();
        Container.Bind<GetCoinsButton>().FromInstance(_getCoinsButton).AsTransient().NonLazy();
        Container.Bind<CoinsHolder>().FromInstance(_coinsHolder).AsSingle().NonLazy();
        Container.Bind<WinCondition>().FromInstance(_winCondition).AsSingle().NonLazy();
    }
}