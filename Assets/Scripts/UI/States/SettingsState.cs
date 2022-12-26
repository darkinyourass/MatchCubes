using UnityEngine;

namespace UI.States
{
    public class SettingsState: UIBaseState
    {
        public SettingsState(UIStateMachine currentStateContext, UIStateFactory stateFactory) : base(currentStateContext, stateFactory)
        {
        }

        public override void EnterState()
        {
            StateContext.SettingsCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        protected override void UpdateState()
        {
            CheckStates();
        }

        protected override void ExitState()
        {
            StateContext.SettingsCanvas.gameObject.SetActive(false);
        }
        
        private void CheckStates()
        {
            if (StateContext.IsResumeButtonPressed)
            {
                SwitchState(StateFactory.Gameplay());
                StateContext.IsResumeButtonPressed = false;
            }

            if (StateContext.IsMenuButtonPressed)
            {
                SwitchState(StateFactory.Menu());
                StateContext.IsMenuButtonPressed = false;
            }
        }
    }
}