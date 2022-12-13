using Common.Gateway;
using Common.Presenter;
using Common.Usecase;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var colorGateway = new ColorGateway();
        var colorUsecase = new ColorUsecase(colorGateway);
        var colorPresenter = gameObject.AddComponent<ColorPresenter>();
        colorPresenter.Initialize(colorUsecase);

        Container.Bind<IColorGateway>().FromInstance(colorGateway).AsTransient().NonLazy();
        Container.Bind<IColorUsecase>().FromInstance(colorUsecase).AsTransient().NonLazy();
        Container.Bind<IColorPresenter>().FromInstance(colorPresenter).AsTransient().NonLazy();
    }
}