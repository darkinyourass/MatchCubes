using System;
using Common.View;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Cubes.ObjectPooling.PopUpTextPool
{
    public class TextPool : ObjectPool<PopUpTextView>
    {
        private TMP_Text _popupText;
        private string _value;
        
        public void SetTextValue(string value)
        {
            var delay = 0.2f;
            // _popupText.text = value;
            SpawnText(value);
            var rectTransform = _popupText.GetComponent<RectTransform>();
            // rectTransform.localScale = Vector3.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.DOScale(1f, 0.7f).SetEase(Ease.OutBack);
            rectTransform.DOAnchorPosY(540.0f, 0.7f).SetEase(Ease.OutQuint);
        }

        private void SpawnText(string value)
        {
            var objectPool = Get();
            _popupText = objectPool.GetComponent<TMP_Text>();
            _popupText.text = value;
            var rectTransform = objectPool.GetComponent<RectTransform>();
            if (objectPool == null) return;
            rectTransform.position = transform.position;
            objectPool.gameObject.SetActive(true);
        }
    }
}