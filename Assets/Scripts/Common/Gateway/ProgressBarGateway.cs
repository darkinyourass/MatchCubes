namespace Common.Gateway
{
    public class ProgressBarGateway: IProgressBarGateway
    {
        private readonly ProgressBarModel _model;


        public ProgressBarGateway()
        {
            _model = new ProgressBarModel();
        }

        public void SetCurrentValue(float value)
        {
            _model.CurrentValue = value;
        }

        public void SetMaxValue(float value)
        {
            _model.MaxValue = value;
        }

        public float GetCurrentValue()
        {
            return _model.CurrentValue;
        }

        public float GetMaxValue()
        {
            return _model.MaxValue;
        }
    }
}