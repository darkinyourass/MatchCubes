using UnityEngine;

namespace UI.States
{
    public class MenuState: UIBaseState
    {
        public MenuState(UIStateMachine currentStateContext, UIStateFactory stateFactory) : base(currentStateContext, stateFactory)
        {
            
        }

        public override void EnterState()
        {
            Time.timeScale = 0;
            StateContext.MenuCanvas.gameObject.SetActive(true);
            StateContext.Timer.IsTimerSet = false;
            StateContext.Timer.gameObject.SetActive(false);
        }

        protected override void UpdateState()
        {
            CheckStates();
        }

        protected override void ExitState()
        {
            StateContext.MenuCanvas.gameObject.SetActive(false);
        }

        private void CheckStates()
        {
            if (StateContext.IsLevelSelected)
            {
                SwitchState(StateFactory.Gameplay());
                StateContext.IsLevelSelected = false;
            }
        }
    }
}