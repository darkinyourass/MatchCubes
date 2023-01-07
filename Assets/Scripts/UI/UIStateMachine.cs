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

        [Header("UI elements")]
        [SerializeField] private LevelTypeChoiceView _playButton;
        [SerializeField] private PopUpTextView _popUpTextView;
        [SerializeField] private Stars _stars;
        [SerializeField] private ProgressBarValue _progressBarValue;

        [Header("Stars values")] 
        [SerializeField] private int _oneStarValue;
        [SerializeField] private int _twoStarValue;
        [SerializeField] private int _threeStarValue;

        
        [field: Header("Level value")]
        [field: SerializeField]
        public static int LevelNumber { get; private set; }
        
        [field: Header("Set time value")]
        [field: SerializeField] 
        public float TimeValue { get; set; }
        
        public PopUpTextView PopTextView => _popUpTextView;
        
        public Timer Timer { get; private set; }

        public float CurrentValue
        {
            get => _currentTimeValue; 
            set => _currentTimeValue = Mathf.Clamp(value, 0, TimeValue);
        }

        private TestGrid Grid { get; set; }

        private WinCondition _winCondition;

        private GetCoinsButton _getCoinsButton;

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
        private void Constructor(Timer timer, TestGrid grid, GetCoinsButton coinsButton)
        {
            Timer = timer;
            Grid = grid;
            _getCoinsButton = coinsButton;
        }

        private void Awake()
        {
            _winCondition = FindObjectOfType<WinCondition>();
            _winCondition.OnAllCubesMatched += OnLevelEnd;
            StateFactory = new UIStateFactory(this);
            CurrentState = StateFactory.Menu();
            CurrentState.EnterState();
            _playButton.OnLevelTypeButtonClicked += OnLevelChose;
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
                _progressBarValue.ResetProgressValue();
            }
            else
            {
                IsLevelLost = false;
            }
        }

        private void OnLevelEnd()
        {
            StartCoroutine(SetGridFalseCo());
            TestGrid.isTutorialFinished = true;
        }


        private IEnumerator SetGridFalseCo()
        {
            yield return new WaitForSeconds(2f);
            _stars.SetStars(_progressBarValue.CurrentValue, _oneStarValue, _twoStarValue, _threeStarValue);
            Grid.gameObject.SetActive(false);
            IsLevelWon = true;
            _progressBarValue.ResetProgressValue();
        }

        private void OnLevelChose()
        {
            CurrentValue = TimeValue;
            IsLevelSelected = true;
            _stars.ResetStars();
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

        public void OnNextLevelButtonClick()
        {
            if (LevelNumber >= 5)
            {
                SceneManager.LoadScene(Random.Range(1, 6));
                LevelNumber += 1;
                _getCoinsButton.gameObject.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene(LevelNumber + 1);
                LevelNumber += 1;
                _getCoinsButton.gameObject.SetActive(true);
            }
            
        }

        public void OnGetCoinsButtonClick()
        {
            _getCoinsButton.gameObject.SetActive(false);
            CoinsHolder.CoinsAmount += 25;
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