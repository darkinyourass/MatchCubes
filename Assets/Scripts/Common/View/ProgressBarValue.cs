using Common.Presenter;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

namespace Common.View
{
    public class ProgressBarValue : MonoBehaviour
    {
        [Inject] 
        private IProgressBarPresenter _progressBarPresenter;

        private TouchMovement _touchMovement;

        [SerializeField] private float _maxValue;
        [SerializeField] private float _currentValue;
        
        private Image _fillImage;
        private TMP_Text _textValue;
        
        public float MaxValue { get; set; }
        private float CurrentValue { get => _currentValue;
            set => _currentValue = Mathf.Clamp(value, 0, _maxValue);
        }
        
        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }
        
        private void Awake()
        {
            _textValue = GetComponentInChildren<TMP_Text>();
            _fillImage = GetComponent<Image>();
            _touchMovement.OnMatchCubesToProgressBar += AddCurrentValue;
        }

        private void Start()
        {
            _progressBarPresenter.SetMaxValue(_maxValue);
            _progressBarPresenter.SetCurrentValue(CurrentValue);
            
            var reactiveProperty = _progressBarPresenter.CurrentValue;
            reactiveProperty.Subscribe(SetValue).AddTo(this);
        }

        private void Update()
        {
            _progressBarPresenter.SetCurrentValue(CurrentValue);
            UpdateFillValue();
        }

        private void AddCurrentValue()
        {
            CurrentValue += 1; 
        }

        private void UpdateFillValue()
        {
            _fillImage.fillAmount = CurrentValue / _maxValue;
        }

        private void SetValue(float value)
        {
            _textValue.text = value + " / " + _maxValue;
        }
    }
}