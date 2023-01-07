using Common.Usecase;
using UniRx;
using UnityEngine;

namespace Common.Presenter
{
    public class ProgressBarPresenter : MonoBehaviour, IProgressBarPresenter
    {
        public IReadOnlyReactiveProperty<int> CurrentValue => _currentValue;
        private readonly ReactiveProperty<int> _currentValue = new ReactiveProperty<int>();


        private IProgressBarUsecase _progressBarUsecase;


        public void Initialize(IProgressBarUsecase progressBarUsecase)
        {
            _progressBarUsecase = progressBarUsecase;
            
            var disposableCurrentValue = _progressBarUsecase.CurrentValue.Subscribe((progressBarModel) =>
            {
               UpdateCurrentValue(progressBarModel); 
            });
            UpdateCurrentValue(_progressBarUsecase.CurrentValue.Value);
        }

        private void UpdateCurrentValue(ProgressBarModel progressBarModel)
        {
            _currentValue.Value = progressBarModel.CurrentValue;
        }

        public void SetCurrentValue(int count)
        {
            _progressBarUsecase.SetCurrentValue(count);
        }

        public void ResetCurrentValue(out int value)
        {
            _progressBarUsecase.ResetCurrentValue(out value);
        }
    }
}