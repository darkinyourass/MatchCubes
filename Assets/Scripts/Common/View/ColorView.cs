using System;
using Common.Presenter;
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

        public ParticleSystem ParticleSystem { get; set; }
        private ParticleSystemRenderer _particleSystemRenderer;

        public int SelectingAnimationHash { get; set; }
        
        public int SpawningAnimationHash { get; set; }

        public MeshRenderer MeshRenderer { get; private set; }
        
        public Animator Animator { get; set; }

        public event Action<ISelectable> OnMouseDownAsButton;

        public bool IsSelected { get; private set; }
        
        public Transform ColorTypeTransform { get; set; }

        public ColorType ColorType => _colorType;

        public LineRenderer LineRenderer { get; set; }

        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            Animator = GetComponent<Animator>();
            LineRenderer = GetComponent<LineRenderer>();
            ColorTypeTransform = GetComponent<Transform>();
            ParticleSystem = GetComponent<ParticleSystem>();
            _particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
            SelectingAnimationHash = Animator.StringToHash("IsSelecting");
            SpawningAnimationHash = Animator.StringToHash("IsSpawning");
            LineRenderer.enabled = false;
        }

        private void Start()
        {
            
            _colorPresenter.SetType(_colorType);
        }

        public void SetColor(int type)
        {
            SetMaterial((ColorType)type);
            _particleSystemRenderer.material = MeshRenderer.material;
        }

        public void Select()
        {
            IsSelected = !IsSelected;
        }

        private void OnMouseUpAsButton()
        {
            OnMouseDownAsButton?.Invoke(this);
        }

        public void SetMaterial(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red:
                    _colorType = _colorPresenter.ColorTypeRed.Value;
                    MeshRenderer.material.SetColor(Color1, new Color32(173, 64, 64, 255));
                    break;
                case ColorType.Green:
                    _colorType = _colorPresenter.ColorTypeGreen.Value;
                    MeshRenderer.material.SetColor(Color1, new Color32(41, 204, 70, 255));
                    break;
                case ColorType.Blue:
                    _colorType = _colorPresenter.ColorTypeBlue.Value;
                    MeshRenderer.material.SetColor(Color1, new Color32(61, 151, 236, 255));
                    break;
                case ColorType.Yellow:
                    _colorType = _colorPresenter.ColorTypeYellow.Value;
                    MeshRenderer.material.SetColor(Color1, new Color32(230, 238, 39, 255));
                    break;
                case ColorType.White:
                    _colorType = _colorPresenter.ColorTypeWhite.Value;
                    MeshRenderer.material.SetColor(Color1, new Color32(255, 255, 255, 255));
                    break;
            }
        }
    }
}