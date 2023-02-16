using TMPro;
using UnityEngine;

namespace Common.View
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;
        public bool IsTimerSet { get; set; }

        private void OnEnable()
        {
            _timerText = GetComponentInChildren<TMP_Text>();
        }

        public void SetTimer(float currentTime)
        {
            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);
            _timerText.text = $"{minutes:00} : {seconds:00}";
        }
    }
}