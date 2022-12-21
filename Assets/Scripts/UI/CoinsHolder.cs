using TMPro;
using UnityEngine;

namespace UI
{
    public class CoinsHolder : MonoBehaviour
    {
        private TMP_Text _coinsText;
        public static int CoinsAmount { get; set; }

        private void Start()
        {
            _coinsText = GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            _coinsText.text = CoinsAmount.ToString();
        }
    }
}