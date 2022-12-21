using Common.Gateway;
using UniRx;

namespace Common.Usecase
{
    public class ProgressBarUsecase: IProgressBarUsecase
    {
        public IReadOnlyReactiveProperty<ProgressBarModel> CurrentValue => _currentValue;
        private readonly ReactiveProperty<ProgressBarModel> _currentValue;
        public IReadOnlyReactiveProperty<ProgressBarModel> MaxValue => _maxValue;
        private readonly ReactiveProperty<ProgressBarModel> _maxValue;

        private readonly IProgressBarGateway _progressBarGateway;

        public ProgressBarUsecase(IProgressBarGateway progressBarGateway)
        {
            _progressBarGateway = progressBarGateway;
            _currentValue = new ReactiveProperty<ProgressBarModel>(new ProgressBarModel());
            _maxValue = new ReactiveProperty<ProgressBarModel>(new ProgressBarModel());
            InitValue();
        }

        public void SetMaxValue(float value)
        {
            _progressBarGateway.SetMaxValue(value);
            var progressBarModel = _maxValue.Value;
            value = _progressBarGateway.GetMaxValue();
            progressBarModel.MaxValue = value;
            _maxValue.SetValueAndForceNotify(progressBarModel);
        }

        public void SetCurrentValue(float value)
        {
            // var value = _progressBarGateway.GetCurrentValue();
            // var newValue = value + count;
            _progressBarGateway.SetCurrentValue(value);
            var progressBarModel = _currentValue.Value;
            value = _progressBarGateway.GetCurrentValue();
            progressBarModel.CurrentValue = value;
            _currentValue.SetValueAndForceNotify(progressBarModel);
        }

        public void ResetCurrentValue(out float value)
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
            var maxValueModel = new ProgressBarModel()
            {
                MaxValue = _progressBarGateway.GetMaxValue()
            };

            var currentValueModel = new ProgressBarModel()
            {
                CurrentValue = _progressBarGateway.GetCurrentValue()
            };

            _currentValue.Value = currentValueModel;
            _maxValue.Value = maxValueModel;
        }
    }
    
    
}