using Common.Gateway;
using UniRx;

namespace Common.Usecase
{
    public class ProgressBarUsecase: IProgressBarUsecase
    {
        public IReadOnlyReactiveProperty<ProgressBarModel> CurrentValue => _currentValue;
        private readonly ReactiveProperty<ProgressBarModel> _currentValue;

        private readonly IProgressBarGateway _progressBarGateway;

        public ProgressBarUsecase(IProgressBarGateway progressBarGateway)
        {
            _progressBarGateway = progressBarGateway;
            _currentValue = new ReactiveProperty<ProgressBarModel>(new ProgressBarModel());
            InitValue();
        }

        public void SetCurrentValue(int value)
        {
            // var value = _progressBarGateway.GetCurrentValue();
            // var newValue = value + count;
            _progressBarGateway.SetCurrentValue(value);
            var progressBarModel = _currentValue.Value;
            value = _progressBarGateway.GetCurrentValue();
            progressBarModel.CurrentValue = value;
            _currentValue.SetValueAndForceNotify(progressBarModel);
        }

        public void ResetCurrentValue(out int value)
        {
            value = 0;
            _progressBarGateway.SetCurrentValue(value);
            var progressBarModel = _currentValue.Value;
            value = _progressBarGateway.GetCurrentValue();
            progressBarModel.CurrentValue = value;
            _currentValue.SetValueAndForceNotify(progressBarModel);
        }

        private void InitValue()
        {
            var currentValueModel = new ProgressBarModel()
            {
                CurrentValue = _progressBarGateway.GetCurrentValue()
            };

            _currentValue.Value = currentValueModel;
        }
    }
    
    
}