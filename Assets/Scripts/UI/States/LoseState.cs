using UnityEngine;

namespace UI.States
{
    public class LoseState: UIBaseState
    {
        public LoseState(UIStateMachine currentStateContext, UIStateFactory stateFactory) : base(currentStateContext, stateFactory)
        {
        }

        public override void EnterState()
        {
            StateContext.LoseCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        protected override void UpdateState()
        {
            CheckStates();
        }

        protected override void ExitState()
        {
            StateContext.LoseCanvas.gameObject.SetActive(false);
        }
        
        private void CheckStates()
        {
            // if (StateContext.IsNextLevelButtonPressed)
            // {
            //     SwitchState(StateFactory.Menu());
            //     StateContext.IsNextLevelButtonPressed = false;
            // }
        }
    }
}