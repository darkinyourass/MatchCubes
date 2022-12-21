using UniRx;

namespace Common.Presenter
{
    public interface IProgressBarPresenter
    {
        public IReadOnlyReactiveProperty<float> CurrentValue { get; }
        public IReadOnlyReactiveProperty<float> MaxValue { get; }

        void SetCurrentValue(float count);
        void SetMaxValue(float value);
        void ResetCurrentValue(out float value);

    }
}