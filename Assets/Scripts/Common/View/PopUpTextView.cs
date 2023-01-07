using System.Collections;
using TMPro;
using UnityEngine;

namespace Common.View
{
    public class PopUpTextView : MonoBehaviour
    {
        private TMP_Text _popupText;
        // public TMP_Text PopupText { get => _popupText; set => _popupText = value; }

        private void OnEnable()
        {
            _popupText = GetComponent<TMP_Text>();
        }

        public void SetTextValue(string value)
        {
            _popupText.text = value;
            StartCoroutine(SetTextFalseCo());
        }

        private IEnumerator SetTextFalseCo()
        {
            yield return new WaitForSeconds(1.5f);
            gameObject.SetActive(false);
        }
    }
}