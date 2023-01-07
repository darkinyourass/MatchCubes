using UniRx;

namespace Common.Presenter
{
    public interface IProgressBarPresenter
    {
        public IReadOnlyReactiveProperty<int> CurrentValue { get; }

        void SetCurrentValue(int count);
        void ResetCurrentValue(out int value);

    }
}