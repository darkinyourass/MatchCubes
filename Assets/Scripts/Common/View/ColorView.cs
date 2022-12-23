using System;
using Common.Presenter;
using UniRx;
using UnityEngine;
using Zenject;

namespace Common.View
{
    public class ColorView : MonoBehaviour, ISelectable
    {
        [Inject] 
        private IColorPresenter _colorPresenter;

        [SerializeField] private ColorType _colorType;
        
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public MeshRenderer MeshRenderer { get; private set; }

        // public event Action<ISelectable> OnMouseDownEvent;
        // public event Action<ISelectable> OnMouseUpEvent;
        // public event Action<ISelectable> OnMouseOverEvent;
        public event Action<ISelectable> OnMouseDownAsButton;

        public bool IsSelected { get; private set; }

        public Transform ColorTypeTransform { get; set; }

        public ColorType ColorType
        {
            get => _colorType;
            set => _colorType = value;
        }

        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
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

            reactiveProperty.Subscribe(SetMaterial).AddTo(this);
        }

        private void Update()
        {
            SetMaterial(_colorType);
        }

        public void Select()
        {
            IsSelected = !IsSelected;
        }

        // private void OnMouseDown()
        // {
        //     OnMouseDownEvent?.Invoke(this);
        // }
        //
        // private void OnMouseOver()
        // {
        //     OnMouseOverEvent?.Invoke(this);
        // }
        //
        // private void OnMouseUp()
        // {
        //     OnMouseUpEvent?.Invoke(this);
        // }

        private void OnMouseUpAsButton()
        {
            OnMouseDownAsButton?.Invoke(this);
        }

        private void SetMaterial(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red:
                    // MeshRenderer.material.SetColor(Color1,
                    //     IsSelected ? new Color32(200, 200, 200, 0) : new Color32(173, 64, 64, 0));
                    MeshRenderer.material.SetColor(Color1,
                        IsSelected ? new Color32(200, 200, 200, 0) : new Color32(173, 64, 64, 0));
                    break;
                case ColorType.Green:
                    MeshRenderer.material.SetColor(Color1,
                        IsSelected ? new Color32(200, 200, 200, 0) : new Color32(41, 204, 70, 255));
                    break;
                case ColorType.Blue:
                    MeshRenderer.material.SetColor(Color1,
                        IsSelected ? new Color32(200, 200, 200, 0) : new Color32(61, 151, 236, 255));
                    break;
                case ColorType.Yellow:
                    MeshRenderer.material.SetColor(Color1,
                        IsSelected ? new Color32(200, 200, 200, 0) : new Color32(230, 238, 39, 255));
                    break;
                case ColorType.White:
                    MeshRenderer.material.SetColor(Color1,
                        IsSelected ? new Color32(200, 200, 200, 0) : new Color32(255, 255, 255, 255));
                    break;
            }
        }
    }
    
    
}