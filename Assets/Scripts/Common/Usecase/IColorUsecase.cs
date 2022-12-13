using System.Collections.Generic;
using UniRx;

namespace Common.Usecase
{
    public interface IColorUsecase
    {
        public IReadOnlyReactiveProperty<Dictionary<ColorType, ColorModel>> Color { get; }
        
        void SetType(ColorType colorType);

    }
}