using System;
using System.Collections;
using Cubes.ObjectPooling.PopUpTextPool;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Common.View
{
    public class PopUpTextView : MonoBehaviour
    {
        public Vector3 _initialPosition;
        private TextPool _textPool;

        private void OnEnable()
        {
            _textPool = FindObjectOfType<TextPool>();
            _initialPosition = gameObject.transform.position;
            StartCoroutine(SetTextFalseCo());
        }

        private void Reset()
        {
            gameObject.transform.position = _initialPosition;
        }

        private IEnumerator SetTextFalseCo()
        {
            yield return new WaitForSeconds(1.5f);
            Reset();
            _textPool.ReturnToPool(this);
            // gameObject.SetActive(false);
        }
    }
}