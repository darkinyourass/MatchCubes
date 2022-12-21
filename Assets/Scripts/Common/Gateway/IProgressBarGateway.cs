namespace Common.Gateway
{
    public interface IProgressBarGateway
    {
        void SetCurrentValue(float value);
        void SetMaxValue(float value);
        
        float GetCurrentValue();
        float GetMaxValue();

    }
}