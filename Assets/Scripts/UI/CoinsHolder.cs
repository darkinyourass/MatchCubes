using System.Collections;
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
            // int targetAmount = CoinsAmount + value;
            // int currentAmount = CoinsAmount;
            //
            // // Calculate the total change in the coin value
            // int change = targetAmount - CoinsAmount;
            //
            // // Calculate the amount to increment the coin value by in each frame
            // int increment = Mathf.CeilToInt((float)change / 120); // 120 frames over 2 seconds
            //
            // yield return new WaitForSeconds(0.5f);
            // var timer = 0f;
            //
            // // Update the coin value in each frame for 2 seconds
            // for (float t = 0; t < 2; t += Time.deltaTime)
            // {
            //     currentAmount += increment;
            //     _coinsText.text = currentAmount.ToString();
            //     yield return null;
            // }

            // for (int i = 0; i < value; i++)
            // {
            //     timer += 0.05f;
            //     _coinsText.text = C.ToString();
            //     yield return new WaitForSeconds(timer);
            // }
            //
            //
            //
            // // Set the final coin amount and save it to PlayerPrefs
            // CoinsAmount += value;
            // PlayerPrefs.SetInt("CoinsAmount", CoinsAmount);
            // PlayerPrefs.Save();
            // _coinsText.text = CoinsAmount.ToString();
            
            int oldValue = CoinsAmount;
            int newValue = CoinsAmount + value;
            StartCoroutine(GradualUpdate(oldValue, newValue, 1f));
        }

        private IEnumerator GradualUpdate(int oldValue, int newValue, float duration)
        {
            
            CoinsAmount = newValue;
            PlayerPrefs.SetInt("CoinsAmount", CoinsAmount);
            PlayerPrefs.Save();
            _coinsText.text = CoinsAmount.ToString();
            
            float timer = 0f;
            while (timer < duration)
            {
                float progress = timer / duration;
                int currentValue = (int)Mathf.Lerp(oldValue, newValue, progress);
                _coinsText.text = currentValue.ToString();
                yield return null;
                timer += Time.deltaTime;
            }
            
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