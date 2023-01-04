using System;
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

        [SerializeField] private float _currentValue;
        
        private Image _fillImage;
        private TMP_Text _textValue;
        
        public float MaxValue { get; set; }
        
        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }
        
        private void Awake()
        {
            _textValue = GetComponentInChildren<TMP_Text>();
            _fillImage = GetComponent<Image>();
            
        }

        private void OnEnable()
        {
            _touchMovement.OnMakeMove += AddCurrentValue;
        }

        private void OnDisable()
        {
            _touchMovement.OnMakeMove -= AddCurrentValue;
        }

        private void Start()
        {
            _progressBarPresenter.SetCurrentValue(_currentValue);
            
            var reactiveProperty = _progressBarPresenter.CurrentValue;
            reactiveProperty.Subscribe(SetValue).AddTo(this);
        }

        private void Update()
        {
            _progressBarPresenter.SetCurrentValue(_currentValue);
        }

        private void AddCurrentValue()
        {
            _currentValue += 1; 
        }

        private void SetValue(float value)
        {
            _textValue.text = $"Moves {value}";
        }
    }
}