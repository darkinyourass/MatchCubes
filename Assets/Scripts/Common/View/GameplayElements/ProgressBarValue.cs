using Common.Presenter;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;


namespace Common.View
{
    public class ProgressBarValue : MonoBehaviour
    {
        [Inject] 
        private IProgressBarPresenter _progressBarPresenter;

        private TouchMovement _touchMovement;

        [SerializeField] private int _currentValue;
        
        public int CurrentValue { get => _currentValue ; set => _currentValue = value; }

        private TMP_Text _textValue;

        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }
        
        private void Awake()
        {
            _textValue = GetComponentInChildren<TMP_Text>();
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

        public void ResetProgressValue()
        {
            _currentValue = 0;
            _progressBarPresenter.ResetCurrentValue(out _currentValue);
        }

        private void SetValue(int value)
        {
            _textValue.text = $"Moves {value}";
        }
    }
}