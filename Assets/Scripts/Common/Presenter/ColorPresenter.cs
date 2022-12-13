using System;
using System.Collections.Generic;
using Common.Usecase;
using UniRx;
using UnityEngine;

namespace Common.Presenter
{
    public class ColorPresenter : MonoBehaviour, IColorPresenter
    {
        public IReadOnlyReactiveProperty<ColorType> ColorTypeRed => _colorTypeRed;
        private readonly ReactiveProperty<ColorType> _colorTypeRed = new ReactiveProperty<ColorType>();
        
        public IReadOnlyReactiveProperty<ColorType> ColorTypeGreen => _colorTypeGreen;
        private readonly ReactiveProperty<ColorType> _colorTypeGreen = new ReactiveProperty<ColorType>();
        
        public IReadOnlyReactiveProperty<ColorType> ColorTypeBlue => _colorTypeBlue;
        private readonly ReactiveProperty<ColorType> _colorTypeBlue = new ReactiveProperty<ColorType>();
        
        public IReadOnlyReactiveProperty<ColorType> ColorTypeYellow => _colorTypeYellow;
        private readonly ReactiveProperty<ColorType> _colorTypeYellow = new ReactiveProperty<ColorType>();
        
        public IReadOnlyReactiveProperty<ColorType> ColorTypeWhite => _colorTypeWhite;
        private readonly ReactiveProperty<ColorType> _colorTypeWhite = new ReactiveProperty<ColorType>();
        
        private IColorUsecase _colorUsecase;

        public void Initialize(IColorUsecase colorUsecase)
        {
            _colorUsecase = colorUsecase;
            var disposable = _colorUsecase.Color.Subscribe((color) =>
            {
                UpdateType(color);
            });
            
            UpdateType(_colorUsecase.Color.Value);
        }

        public void SetType(ColorType colorType)
        {
            _colorUsecase.SetType(colorType);
        }
        
        private void UpdateType(Dictionary<ColorType, ColorModel> dictionary)
        {
            foreach (var (key, value) in dictionary)
            {
                switch (key)
                {
                    case ColorType.Red:
                        _colorTypeRed.Value = value.ColorType;
                        break;
                    case ColorType.Green:
                        _colorTypeGreen.Value = value.ColorType;
                        break;
                    case ColorType.Blue:
                        _colorTypeBlue.Value = value.ColorType;
                        break;
                    case ColorType.Yellow:
                        _colorTypeYellow.Value = value.ColorType;
                        break;
                    case ColorType.White:
                        _colorTypeWhite.Value = value.ColorType;
                        break;
                }
            }
        }
    }
}