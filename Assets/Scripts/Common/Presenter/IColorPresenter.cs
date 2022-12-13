using UniRx;

namespace Common.Presenter
{
    public interface IColorPresenter
    {
        IReadOnlyReactiveProperty<ColorType> ColorTypeRed { get; }
        
        IReadOnlyReactiveProperty<ColorType> ColorTypeGreen { get; }
        
        IReadOnlyReactiveProperty<ColorType> ColorTypeBlue { get; }
        
        IReadOnlyReactiveProperty<ColorType> ColorTypeYellow { get; }
        
        IReadOnlyReactiveProperty<ColorType> ColorTypeWhite { get; }
        
        void SetType(ColorType colorType);
    }
}