using System;
using Common.Presenter;
using UniRx;
using UnityEngine;
using Zenject;

namespace Common.View
{
    public class ColorView : MonoBehaviour, ISelectable
    {
        [Inject] private IColorPresenter _colorPresenter;

        [SerializeField] private ColorType _colorType;
        
        private MeshRenderer _meshRenderer;

        public bool IsSelected { get; set; } = false;
        public Transform ColorTypeTransform { get; set; }

        public ColorType ColorType
        {
            get => _colorType;
            set => _colorType = value;
        }

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            ColorTypeTransform = GetComponent<Transform>();
        }

        private void Start()
        {
            _colorPresenter.SetType(_colorType);
            var reactiveProperty = _colorType switch
            {
                ColorType.Red => _colorPresenter.ColorTypeRed,
                ColorType.Green => _colorPresenter.ColorTypeGreen,
                ColorType.Blue => _colorPresenter.ColorTypeBlue,
                ColorType.Yellow => _colorPresenter.ColorTypeYellow,
                _ => _colorPresenter.ColorTypeWhite
            };

            reactiveProperty.Subscribe((type) => { SetMaterial(type); }).AddTo(this);
        }

        private void Update()
        {
            SetMaterial(_colorType);
        }

        public void Select()
        {
            IsSelected = !IsSelected;
        }

        private void SetMaterial(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red:
                    if (IsSelected)
                    {
                        _meshRenderer.material.SetColor( "_Color", new Color32(200, 200, 200, 0));
                    }
                    else
                    {
                        _meshRenderer.material.SetColor("_Color", new Color32(173, 64, 64, 255));
                    }
                    break;
                case ColorType.Green:
                    if (IsSelected)
                    {
                        _meshRenderer.material.SetColor( "_Color", new Color32(200, 200, 200, 0));
                    }
                    else
                    {
                        _meshRenderer.material.SetColor("_Color", new Color32(41, 204, 70, 255));
                    }
                    break;
                case ColorType.Blue:
                    if (IsSelected)
                    {
                        _meshRenderer.material.SetColor( "_Color", new Color32(200, 200, 200, 0));
                    }
                    else
                    {
                        _meshRenderer.material.SetColor("_Color", new Color32(61, 151, 236, 255));
                    }
                    break;
                case ColorType.Yellow:
                    if (IsSelected)
                    {
                        _meshRenderer.material.SetColor( "_Color", new Color32(200, 200, 200, 0));
                    }
                    else
                    {
                        _meshRenderer.material.SetColor("_Color", new Color32(230, 238, 39, 255));
                    }
                    break;
                case ColorType.White:
                    if (IsSelected)
                    {
                        _meshRenderer.material.SetColor( "_Color", new Color32(200, 200, 200, 0));
                    }
                    else
                    {
                        _meshRenderer.material.SetColor("_Color", new Color32(255, 255, 255, 255));
                    }
                    break;
            }
        }
    }
}