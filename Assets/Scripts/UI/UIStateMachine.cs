using System.Collections;
using Common.View;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cubes;

namespace UI
{
    public class UIStateMachine : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] private Canvas _settingsCanvas;
        [SerializeField] private Canvas _gamePlayCanvas;
        [SerializeField] private Canvas _winCanvas;
        [SerializeField] private Canvas _menuCanvas;
        [SerializeField] private Canvas _loseCanvas;
        // [SerializeField] private AudioClip _audioClip;

        private float _currentTimeValue;

        [Header("Level type buttons")]
        [SerializeField] private LevelTypeChoiceView _levelType;

        public Timer Timer { get; private set; }

        [field: SerializeField] public float TimeValue { get; set; }
        public float CurrentValue
        {
            get => _currentTimeValue; 
            set => _currentTimeValue = Mathf.Clamp(value, 0, TimeValue);
        }

        private TestGrid Grid { get; set; }
        

        private WinCondition _winCondition;

        public bool IsResumeButtonPressed { get; set; }
        
        public bool IsSettingsButtonPressed { get; set; }
        
        public bool IsMenuButtonPressed { get; set; }
        
        public bool IsLevelWon { get; set; }
        
        public bool IsLevelLost { get; set; }
        
        public bool IsLevelSelected { get; set; }

        public Canvas MenuCanvas => _menuCanvas;

        public Canvas SettingsCanvas => _settingsCanvas;

        public Canvas GamePlayCanvas => _gamePlayCanvas;

        public Canvas WinCanvas => _winCanvas;

        public Canvas LoseCanvas => _loseCanvas;

        public UIBaseState CurrentState { get; set; }
        private UIStateFactory StateFactory { get; set; }

        [Inject]
        private void Constructor(Timer timer, TestGrid grid)
        {
            Timer = timer;
            Grid = grid;
        }

        private void Awake()
        {
            _winCondition = FindObjectOfType<WinCondition>();
            _winCondition.OnAllCubesMatched += OnLevelEnd;
            StateFactory = new UIStateFactory(this);
            CurrentState = StateFactory.Menu();
            CurrentState.EnterState();
            _levelType.OnLevelTypeButtonClicked += OnLevelChose;
            
        }

        private void Update()
        {
            IsLost();
            CurrentState.UpdateStates();
        }

        private void FixedUpdate()
        {
            if (!Timer.IsTimerSet) return;
            CurrentValue -= Time.fixedDeltaTime;
            Timer.SetTimer(CurrentValue);
        }

        private void IsLost()
        {
            if (CurrentValue <= 0)
            {
                IsLevelLost = true;
                Timer.IsTimerSet = false;
                Grid.gameObject.SetActive(false);
            }
            else
            {
                IsLevelLost = false;
            }
        }

        private void OnLevelEnd()
        {
            StartCoroutine(SetGridFalseCo());
        }


        private IEnumerator SetGridFalseCo()
        {
            yield return new WaitForSeconds(2f);
            Grid.gameObject.SetActive(false);
            IsLevelWon = true;
        }

        private void OnLevelChose()
        {
            CurrentValue = TimeValue;
            IsLevelSelected = true;
        }

        public void OnCancelButtonClick()
        {
            IsResumeButtonPressed = true;
        }

        public void OnMenuButtonClick()
        {
            IsMenuButtonPressed = true;
            Grid.gameObject.SetActive(false);
        }

        public void OnSettingsButtonClick()
        {
            IsSettingsButtonPressed = true;
        }

        public void OnNextLevelButtonClick(int sceneID)
        {
            SceneManager.LoadScene(sceneID);
        }

        public void OnGetCoinsButtonClick()
        {
            CoinsHolder.CoinsAmount += 100;
        }
        
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // public void ToggleSound()
        // {
        //     IsSoundToggle = IsSoundToggle switch
        //     {
        //         true => false,
        //         false => true
        //     };
        // }
        //
        // public void ToggleVibration()
        // {
        //     IsVibrationToggle = IsVibrationToggle switch
        //     {
        //         true => false,
        //         false => true
        //     };
        // }
    }
}