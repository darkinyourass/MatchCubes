using DefaultNamespace;
using UnityEngine;

namespace UI.States
{
    public class GameplayState: UIBaseState
    {
        public GameplayState(UIStateMachine currentStateContext, UIStateFactory stateFactory) : base(currentStateContext, stateFactory)
        {
        }

        public override void EnterState()
        {
            StateContext.GamePlayCanvas.gameObject.SetActive(true);
            Time.timeScale = 1;
            // if (UIStateMachine.IsSoundToggle)
            // {
            //     AudioManager.Instance.PlayMusic(StateContext.AudioClip);
            // }
        }

        protected override void UpdateState()
        {
            CheckStates();
        }

        protected override void ExitState()
        {
            Time.timeScale = 0;
            AudioManager.Instance.MusicSource.Stop();
        }
        
        private void CheckStates()
        {
            if (StateContext.IsLevelWon)
            {
                StateContext.GamePlayCanvas.gameObject.SetActive(false);
                SwitchState(StateFactory.Win());
                StateContext.IsLevelWon = false;
            }
            if (StateContext.IsLevelLost)
            {
                StateContext.GamePlayCanvas.gameObject.SetActive(false);
                SwitchState(StateFactory.Lose());
                StateContext.IsLevelLost = false;
            }
            if (StateContext.IsSettingsButtonPressed)
            {
                SwitchState(StateFactory.Settings());
                StateContext.IsSettingsButtonPressed = false;
            }
        }
    }
}