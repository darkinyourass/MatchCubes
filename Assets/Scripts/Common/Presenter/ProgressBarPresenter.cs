using Common.Usecase;
using UniRx;
using UnityEngine;

namespace Common.Presenter
{
    public class ProgressBarPresenter : MonoBehaviour, IProgressBarPresenter
    {
        public IReadOnlyReactiveProperty<float> CurrentValue => _currentValue;
        private readonly ReactiveProperty<float> _currentValue = new ReactiveProperty<float>();
        
        public IReadOnlyReactiveProperty<float> MaxValue => _maxValue;
        private readonly ReactiveProperty<float> _maxValue = new ReactiveProperty<float>();

        private IProgressBarUsecase _progressBarUsecase;


        public void Initialize(IProgressBarUsecase progressBarUsecase)
        {
            _progressBarUsecase = progressBarUsecase;
            
            var disposableCurrentValue = _progressBarUsecase.CurrentValue.Subscribe((progressBarModel) =>
            {
               UpdateCurrentValue(progressBarModel); 
            });
            UpdateCurrentValue(_progressBarUsecase.CurrentValue.Value);
            
            var disposableMaxValue = _progressBarUsecase.MaxValue.Subscribe((progressBarModel) =>
            {
                UpdateMaxValue(progressBarModel); 
            });
            UpdateMaxValue(_progressBarUsecase.MaxValue.Value);
            
        }

        private void UpdateCurrentValue(ProgressBarModel progressBarModel)
        {
            _currentValue.Value = progressBarModel.CurrentValue;
        }

        private void UpdateMaxValue(ProgressBarModel progressBarModel)
        {
            _maxValue.Value = progressBarModel.MaxValue;
        }

        public void SetCurrentValue(float count)
        {
            _progressBarUsecase.SetCurrentValue(count);
        }

        public void SetMaxValue(float value)
        {
            _progressBarUsecase.SetMaxValue(value);
        }

        public void ResetCurrentValue(out float value)
        {
            _progressBarUsecase.ResetCurrentValue(out value);
        }
    }
}