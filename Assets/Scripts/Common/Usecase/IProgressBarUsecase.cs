using UniRx;

namespace Common.Usecase
{
    public interface IProgressBarUsecase
    {
        IReadOnlyReactiveProperty<ProgressBarModel> CurrentValue { get; }
        IReadOnlyReactiveProperty<ProgressBarModel> MaxValue { get; }
        
        public void SetMaxValue(float value);
        public void SetCurrentValue(float count);
        public void ResetCurrentValue(out float value);

    }
}