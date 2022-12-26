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
        
        [Header("Time")]
        [SerializeField] private float _timeValue;
        [SerializeField] private float _currentTimeValue;

        [Header("Level type buttons")]
        [SerializeField] private LevelTypeChoiceView[] _levelTypes;

        public Timer Timer { get; private set; }

        private float CurrentValue
        {
            get => _currentTimeValue; 
            set => _currentTimeValue = Mathf.Clamp(value, 0, _timeValue);
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
            foreach (var levelType in _levelTypes)
            {
                levelType.OnLevelTypeButtonClicked += OnLevelChose;
            }
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
            IsLevelWon = true;
            Grid.gameObject.SetActive(false);
        }

        private void OnLevelChose()
        {
            CurrentValue = _timeValue;
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