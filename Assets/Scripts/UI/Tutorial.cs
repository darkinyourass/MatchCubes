using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _handImage;
        [SerializeField] private TMP_Text _text;

        private void OnEnable()
        {
            _handImage = GetComponent<SpriteRenderer>();
        }

        public void SetHandImagePosition(Vector3 position)
        {
            _handImage.gameObject.SetActive(true);
            _handImage.transform.position = position;
        }

        public void SetTutorialText(String message)
        {
            _text.text = message;
        }
    }
}