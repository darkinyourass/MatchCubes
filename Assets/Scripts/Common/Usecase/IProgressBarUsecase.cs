using UniRx;

namespace Common.Usecase
{
    public interface IProgressBarUsecase
    {
        IReadOnlyReactiveProperty<ProgressBarModel> CurrentValue { get; }

        public void SetCurrentValue(int count);
        public void ResetCurrentValue(out int value);

    }
}