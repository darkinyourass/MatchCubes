using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.View
{
    public class Stars : MonoBehaviour
    {
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private Image _star3;

        public void ResetStars()
        {
            _star1.gameObject.SetActive(false);
            _star2.gameObject.SetActive(false);
            _star3.gameObject.SetActive(false);
        }

        public void SetStars(int totalMoves, int one, int two, int three)
        {
            if (totalMoves <= one)
            {
                _star1.gameObject.SetActive(true);
            }
            if (totalMoves <= two)
            {
                _star2.gameObject.SetActive(true);
            }
            if (totalMoves <= three)
            {
                _star3.gameObject.SetActive(true);
            }
        }
    }
}