using System;

namespace Common.Gateway
{
    public class ColorGateway: IColorGateway
    {
        private readonly ColorModel _colorRed;
        private readonly ColorModel _colorGreen;
        private readonly ColorModel _colorBlue;
        private readonly ColorModel _colorYellow;
        private readonly ColorModel _colorWhite;

        public ColorGateway()
        {
            _colorRed = new ColorModel();
            {
                _colorRed.ColorType = ColorType.Red;
            }
            
            _colorGreen = new ColorModel();
            {
                _colorGreen.ColorType = ColorType.Green;
            }
            
            _colorBlue = new ColorModel();
            {
                _colorBlue.ColorType = ColorType.Blue;
            }

            _colorYellow = new ColorModel();
            {
                _colorYellow.ColorType = ColorType.Yellow;
            }

            _colorWhite = new ColorModel();
            {
                _colorWhite.ColorType = ColorType.White;
            }
        }

        public void SetColorValue(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.White:
                    _colorWhite.ColorType = ColorType.White;
                    break;
                case ColorType.Red:
                    _colorRed.ColorType = ColorType.Red;
                    break;
                case ColorType.Green:
                    _colorGreen.ColorType = ColorType.Green;
                    break;
                case ColorType.Blue:
                    _colorBlue.ColorType = ColorType.Blue;
                    break;
                case ColorType.Yellow:
                    _colorYellow.ColorType = ColorType.Yellow;
                    break;
            }
        }

        public ColorType GetColorType(ColorType colorType)
        {
            return colorType switch
            {
                ColorType.Red => _colorRed.ColorType = ColorType.Red,
                ColorType.Green => _colorGreen.ColorType = ColorType.Green,
                ColorType.Blue => _colorBlue.ColorType = ColorType.Blue,
                ColorType.Yellow => _colorYellow.ColorType = ColorType.Yellow,
                _ => _colorWhite.ColorType = ColorType.White
            };
        }
    }
}