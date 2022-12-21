using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIStateMachine : MonoBehaviour
    {
        [SerializeField] private Canvas _settingsCanvas;
        [SerializeField] private Canvas _gamePlayCanvas;
        [SerializeField] private Canvas _winCanvas;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField] private TMP_Text _coinsText;

        private WinCondition _winCondition;

        public bool IsResumeButtonPressed { get; set; }
        public bool IsSettingsButtonPressed { get; set; }
        public bool IsLevelEnded { get; set; }

        // public static bool IsSoundToggle { get; set; }
        // public static bool IsVibrationToggle { get; set; }
        // public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }

        // public Canvas MenuCanvas => _menuCanvas;

        public Canvas SettingsCanvas => _settingsCanvas;

        public Canvas GamePlayCanvas => _gamePlayCanvas;

        public Canvas WinCanvas => _winCanvas;

        public UIBaseState CurrentState { get; set; }
        private UIStateFactory StateFactory { get; set; }

        private void Awake()
        {
            _winCondition = FindObjectOfType<WinCondition>();
            _winCondition.OnAllCubesMatched += OnLevelEnd;
            StateFactory = new UIStateFactory(this);
            CurrentState = StateFactory.Gameplay();
            CurrentState.EnterState();
        }

        private void Update()
        {
            CurrentState.UpdateStates();
        }

        private void OnLevelEnd()
        {
            IsLevelEnded = true;
        }

        public void OnCancelButtonClick()
        {
            IsResumeButtonPressed = true;
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

        public void OnCameraLockButtonClick()
        {
            CameraMovement.IsCameraLocked = CameraMovement.IsCameraLocked switch
            {
                true => false,
                false => true
            };
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