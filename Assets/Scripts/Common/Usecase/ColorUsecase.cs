using System.Collections.Generic;
using Common.Gateway;
using UniRx;

namespace Common.Usecase
{
    public class ColorUsecase: IColorUsecase
    {
        public IReadOnlyReactiveProperty<Dictionary<ColorType, ColorModel>> Color => _color;
        private ReactiveProperty<Dictionary<ColorType, ColorModel>> _color;

        private readonly IColorGateway _colorGateway;

        public ColorUsecase(IColorGateway colorGateway)
        {
            _colorGateway = colorGateway;
            _color = new ReactiveProperty<Dictionary<ColorType, ColorModel>>(new Dictionary<ColorType, ColorModel>());
            
            InitType(ColorType.Red);
            InitType(ColorType.Green);
            InitType(ColorType.Blue);
            InitType(ColorType.Yellow);
            InitType(ColorType.White);
        }

        public void SetType(ColorType colorType)
        {
            _colorGateway.SetColorValue(colorType);
            var dictionary = _color.Value;
            colorType = _colorGateway.GetColorType(colorType);
            dictionary[colorType].ColorType = colorType;
            _color.SetValueAndForceNotify(dictionary);
        }

        private void InitType(ColorType colorType)
        {
            var type = new ColorModel()
            {
                ColorType = _colorGateway.GetColorType(colorType)
            };
            _color.Value.Add(colorType, type);
        }
    }
}