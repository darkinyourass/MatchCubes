using System;
using System.Collections.Generic;
using Common.View;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class CubesLayer : MonoBehaviour
    {
        private readonly List<ColorView> _cubesInLayerCount = new ();
        private ColorView[] _colorViews;

        private TouchMovement _touchMovement;
        
        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }
        
        private void Awake()
        {
            _colorViews = GetComponentsInChildren<ColorView>();
            _cubesInLayerCount.AddRange(_colorViews);
            _touchMovement.OnMatchCubes += CheckLayer;
        }

        private bool IsLayerEmpty()
        {
            return _cubesInLayerCount.Count == 0;
        }

        private void CheckLayer(ISelectable first, ISelectable second)
        {
            _cubesInLayerCount.Remove((ColorView)first);
            _cubesInLayerCount.Remove((ColorView)second);
            if (IsLayerEmpty())
            {
                gameObject.SetActive(false);
            }
        }
    }
}