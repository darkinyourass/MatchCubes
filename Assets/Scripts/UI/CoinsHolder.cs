using TMPro;
using UnityEngine;

namespace UI
{
    public class CoinsHolder : MonoBehaviour
    {

        private TMP_Text _coinsText;
        private int CoinsAmount { get; set; }

        private void Start()
        {
            CoinsAmount = PlayerPrefs.GetInt("CoinsAmount", 0);
            _coinsText = GetComponentInChildren<TMP_Text>();
            _coinsText.text = CoinsAmount.ToString();
        }

        public void UpdateValue(int value)
        {
            CoinsAmount += value;
            PlayerPrefs.SetInt("CoinsAmount", CoinsAmount);
            PlayerPrefs.Save();
            _coinsText.text = CoinsAmount.ToString();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            switch (pauseStatus)
            {
                case true:
                    PlayerPrefs.SetInt("CoinsAmount", CoinsAmount);
                    PlayerPrefs.Save();
                    break;
                case false:
                    CoinsAmount = PlayerPrefs.GetInt("CoinsAmount", 0);
                    _coinsText = GetComponentInChildren<TMP_Text>();
                    _coinsText.text = CoinsAmount.ToString();
                    break;
            }
        }
    }
}