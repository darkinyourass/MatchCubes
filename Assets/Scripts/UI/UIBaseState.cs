namespace UI
{
    public abstract class UIBaseState
    {
        protected UIStateFactory StateFactory { get; set; }
        protected UIStateMachine StateContext { get; set; }
        
        protected UIBaseState(UIStateMachine currentStateContext, UIStateFactory stateFactory)
        {
            StateContext = currentStateContext;
            StateFactory = stateFactory;
        }

        public abstract void EnterState();
        protected abstract void UpdateState();
        protected abstract void ExitState();

        public void UpdateStates()
        {
            UpdateState();
        }

        protected void SwitchState(UIBaseState newState)
        {
            ExitState();
            newState.EnterState();
            StateContext.CurrentState = newState;
        }

    }
}