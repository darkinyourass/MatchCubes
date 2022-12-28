using System;
using System.Collections.Generic;
using Common.View;
using UnityEngine;
using Zenject;

namespace Cubes
{
    public class WinCondition : MonoBehaviour
    {
        [SerializeField] private List<ISelectable> _colorViews = new ();

        public event Action OnAllCubesMatched;
        
        private TouchMovement _touchMovement;

        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }

        private void Awake()
        {
            // _touchMovement.OnMatchCubes += AddToList;
            _touchMovement.OnMatchingCubes += AddToList;
        }

        // private void AddToList(ISelectable first, ISelectable second)
        // {
        //     _colorViews.Add((ColorView)first);
        //     _colorViews.Add((ColorView)second);
        //     if (_colorViews.Count != _touchMovement._colorViews.Count) return;
        //     OnAllCubesMatched?.Invoke();
        //     _touchMovement.EmptySelectables.RemoveRange(0, _touchMovement.EmptySelectables.Count);
        // }
        
        private void AddToList(List<ISelectable> selectables)
        {
            _colorViews.AddRange(selectables);
            if (_colorViews.Count != _touchMovement._colorViews.Count) return;
            OnAllCubesMatched?.Invoke();
            _touchMovement.EmptySelectables.RemoveRange(0, _touchMovement.EmptySelectables.Count);
        }
    }
}