using System;
using System.Collections.Generic;
using Common.View;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class WinCondition : MonoBehaviour
    {
        [SerializeField] private List<ColorView> _colorViews = new List<ColorView>();

        public event Action OnAllCubesMatched;
        
        private TouchMovement _touchMovement;

        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }

        private void Awake()
        {
            // _touchMovement = FindObjectOfType<TouchMovement>();
            _touchMovement.OnMatchCubes += AddToList;
        }

        private void AddToList(ISelectable first, ISelectable second)
        {
            _colorViews.Add((ColorView)first);
            _colorViews.Add((ColorView)second);
            if (_colorViews.Count == _touchMovement.ColorViews.Length)
            {
                OnAllCubesMatched?.Invoke();
            }
        }
        
        
        
    }
}