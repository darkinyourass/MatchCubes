namespace Common.Gateway
{
    public class ProgressBarGateway: IProgressBarGateway
    {
        private readonly ProgressBarModel _model;


        public ProgressBarGateway()
        {
            _model = new ProgressBarModel();
        }

        public void SetCurrentValue(int value)
        {
            _model.CurrentValue = value;
        }

        public int GetCurrentValue()
        {
            return _model.CurrentValue;
        }
    }
}