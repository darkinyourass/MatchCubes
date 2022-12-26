using System.Collections.Generic;
using UI.Enum;
using UI.States;

namespace UI
{
    public class UIStateFactory
    {
        private UIStateMachine _context;

        private readonly Dictionary<UIStates, UIBaseState> _states = new Dictionary<UIStates, UIBaseState>();

        public UIStateFactory(UIStateMachine uiStateMachine)
        {
            _context = uiStateMachine;

            _states[UIStates.Menu] = new MenuState(_context, this);
            _states[UIStates.Gameplay] = new GameplayState(_context, this);
            _states[UIStates.Settings] = new SettingsState(_context, this);
            _states[UIStates.Win] = new WinState(_context, this);
            _states[UIStates.Lose] = new LoseState(_context, this);
            // _states[UIStates.ChooseBonus] = new ChooseBonusState(_context, this);
        }

        public UIBaseState Menu()
        {
            return _states[UIStates.Menu];
        }
        
        public UIBaseState Gameplay()
        {
            return _states[UIStates.Gameplay];
        }
        
        public UIBaseState Settings()
        {
            return _states[UIStates.Settings];
        }
        
        public UIBaseState Win()
        {
            return _states[UIStates.Win];
        }

        public UIBaseState Lose()
        {
            return _states[UIStates.Lose];
        }

        // public UIBaseState ChooseBonus()
        // {
        //     return _states[UIStates.ChooseBonus];
        // }
    }
}