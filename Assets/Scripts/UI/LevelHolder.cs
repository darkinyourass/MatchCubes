using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class LevelHolder : MonoBehaviour
    {
        private TMP_Text _levelNumberText;
        
        private UIStateMachine _stateMachine;
        
        [Inject]
        private void Constructor(UIStateMachine uiStateMachine)
        {
            _stateMachine = uiStateMachine;
        }

        private void OnEnable()
        {
            _levelNumberText = GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            _levelNumberText.text = _stateMachine.LevelNumber == 0 ? "" : $"Level {_stateMachine.LevelNumber}";
        }
    }
}