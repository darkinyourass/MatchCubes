namespace Common.Gateway
{
    public interface IProgressBarGateway
    {
        void SetCurrentValue(int value);
        int GetCurrentValue();
    }
}