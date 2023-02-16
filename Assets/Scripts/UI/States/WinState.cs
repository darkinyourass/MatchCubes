using UnityEngine;

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
            // Time.timeScale = 0;
        }

        protected override void UpdateState()
        {
            CheckStates();
        }

        protected override void ExitState()
        {
            StateContext.WinCanvas.gameObject.SetActive(false);
        }
        
        private void CheckStates()
        {
            if (StateContext.IsNextLevelButtonPressed)
            {
                SwitchState(StateFactory.Gameplay());
                StateContext.IsNextLevelButtonPressed = false;
            }
        }
    }
}