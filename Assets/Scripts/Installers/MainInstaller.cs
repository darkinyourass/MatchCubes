using Common.Gateway;
using Common.Presenter;
using Common.Usecase;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private TouchMovement _touchMovement;
    
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
    }

    private void BindTouchMovement()
    {
        Container.Bind<TouchMovement>().FromInstance(_touchMovement).AsSingle().NonLazy();
    }
}