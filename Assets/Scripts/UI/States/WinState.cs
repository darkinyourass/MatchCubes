namespace UI.States
{
    public class WinState: UIBaseState
    {
        public WinState(UIStateMachine currentStateContext, UIStateFactory stateFactory) : base(currentStateContext, stateFactory)
        {
        }

        public override void EnterState()
        {
            StateContext.WinCanvas.gameObject.SetActive(true);
        }

        protected override void UpdateState()
        {
            
        }

        protected override void ExitState()
        {
            StateContext.WinCanvas.gameObject.SetActive(false);
        }
    }
}