using System;
using System.Collections;
using Common.View;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cubes;
using Random = UnityEngine.Random;

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

        [Header("UI elements")]
        [SerializeField] private LevelTypeChoiceView _playButton;
        // [SerializeField] private PopUpTextView _popUpTextView;
        [SerializeField] private Stars _stars;
        [SerializeField] private ProgressBarValue _progressBarValue;
        [SerializeField] private GameObject _tapToPlayBackground;

        [Header("Stars values")] 
        [SerializeField] private int _oneStarValue;
        [SerializeField] private int _twoStarValue;
        [SerializeField] private int _threeStarValue;

        [field: Header("Level value")]
        [field: SerializeField]
        public int LevelNumber { get; set; }
        private int SceneIndex { get; set; }
        [field: SerializeField] private bool IsLoadingDone { get; set; }
        
        [field: Header("Set time value")]
        [field: SerializeField] 
        public float TimeValue { get; set; }
        private float _currentTimeValue;

        public event Action OnBombButtonClick;

        // public PopUpTextView PopTextView => _popUpTextView;
        
        public Timer Timer { get; private set; }

        public float CurrentValue { get => _currentTimeValue; set => _currentTimeValue = Mathf.Clamp(value, 0, TimeValue); }

        private TestGrid Grid { get; set; }

        private WinCondition _winCondition;

        private GetCoinsButton _getCoinsButton;

        private CoinsHolder _coinsHolder;

        private TouchMovement _touchMovement;

        public bool IsResumeButtonPressed { get; set; }
        
        public bool IsSettingsButtonPressed { get; set; }
        
        public bool IsNextLevelButtonPressed { get; set; }
        
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
        private void Constructor(Timer timer, TestGrid grid, GetCoinsButton coinsButton, CoinsHolder coinsHolder, TouchMovement touchMovement)
        {
            Timer = timer;
            Grid = grid;
            _getCoinsButton = coinsButton;
            _coinsHolder = coinsHolder;
            _touchMovement = touchMovement;
        }

        private void Awake()
        {
            // _winCanvas.gameObject.SetActive(false);
            _winCondition = FindObjectOfType<WinCondition>();
            _winCondition.OnAllCubesMatched += OnLevelEnd;
            StateFactory = new UIStateFactory(this);
            CurrentState = StateFactory.Gameplay();
            CurrentState.EnterState();
            _playButton.OnLevelTypeButtonClicked += OnLevelChose;
            _stars.ResetStars();
        }
        
        private IEnumerator Start()
        {
            SceneIndex = PlayerPrefs.GetInt("SceneIndex", 0);
            LevelNumber = PlayerPrefs.GetInt("LevelNumber", 0);
            int boolValue = PlayerPrefs.GetInt("IsLoading", 0);
            IsLoadingDone = boolValue != 0;
            if (!IsLoadingDone)
            {
                IsLoadingDone = true;
                PlayerPrefs.SetInt("IsLoading", 1);
                PlayerPrefs.Save(); 
                SceneManager.LoadScene(SceneIndex);
            }
            yield return null;
        }

        private void Update()
        {
            Debug.Log($"Current scene {SceneIndex}");

            if (Input.touchCount > 0)
            {
                _tapToPlayBackground.SetActive(false);
            }
            
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
        }

        private IEnumerator SetGridFalseCo()
        {
            const float delay = 2f;
            yield return new WaitForSeconds(delay);
            _stars.SetStars(_progressBarValue.CurrentValue, _oneStarValue, _twoStarValue, _threeStarValue);
            Grid.gameObject.SetActive(false);
            IsLevelWon = true;
            _progressBarValue.ResetProgressValue();
        }

        private void OnLevelChose()
        {
            CurrentValue = TimeValue;
            IsLevelSelected = true;
            
        }

        private void OnApplicationQuit()
        {
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            UpdateSceneValue(currentScene);
            SceneIndex = currentScene;
            PlayerPrefs.SetInt("IsLoading", 0);
            PlayerPrefs.SetInt("SceneIndex", SceneIndex);
            PlayerPrefs.Save();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            switch (pauseStatus)
            {
                case true:
                    var currentScene = SceneManager.GetActiveScene().buildIndex;
                    UpdateSceneValue(currentScene);
                    SceneIndex = currentScene;
                    PlayerPrefs.SetInt("IsLoading", 0);
                    PlayerPrefs.SetInt("SceneIndex", SceneIndex);
                    PlayerPrefs.Save();
                    break;
                case false:
                    SceneIndex = PlayerPrefs.GetInt("SceneIndex", 0);
                    break;
            }
        }

        public void OnCancelButtonClick()
        {
            IsResumeButtonPressed = true;
        }

        public void OnMenuButtonClick()
        {
            IsNextLevelButtonPressed = true;
            Grid.gameObject.SetActive(false);
        }

        public void OnSettingsButtonClick()
        {
            IsSettingsButtonPressed = true;
        }

        public void OnNextLevelButtonClick()
        {
            // Grid.gameObject.SetActive(false);
            IsNextLevelButtonPressed = true;
            UpdateLevelValue(1);
            Grid.UpdateValue(true);
            // _getCoinsButton.gameObject.SetActive(true);
            StartCoroutine(SetButtonTrueCo());
            SceneManager.LoadScene(LevelNumber > 5 ? Random.Range(1, 6) : LevelNumber);
            PlayerPrefs.SetInt("LevelNumber", LevelNumber);
            PlayerPrefs.Save();
        }

        private IEnumerator SetButtonTrueCo()
        {
            const float delay = 0.1f; 
            yield return new WaitForSeconds(delay);
            _getCoinsButton.gameObject.SetActive(true);
        }

        private void UpdateLevelValue(int value)
        {
            LevelNumber += value;
        }

        private void UpdateSceneValue(int index)
        {
            SceneIndex = index;
        }

        public void OnGetCoinsButtonClick()
        {
            _getCoinsButton.gameObject.SetActive(false);
            _coinsHolder.UpdateValue(25);
        }

        public void OnBombBonusClick()
        {
            if (!_touchMovement.IsSelectingCubes)
            {
                // _coinsHolder.UpdateValue(-75);
                OnBombButtonClick?.Invoke();
            }
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