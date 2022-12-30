using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.View;
using DefaultNamespace;
using UI;
using UnityEngine;
using Zenject;

namespace Cubes
{
    public class WinCondition : MonoBehaviour
    {
        private readonly List<ISelectable> _emptySelectables = new ();
        public event Action OnAllCubesMatched;
        
        private TouchMovement _touchMovement;

        [SerializeField] private int _counter;
        
        private TestGrid _testGrid;

        private UIStateMachine _stateMachine;

        [Inject]
        private void Constructor(TestGrid testGrid)
        {
            _testGrid = testGrid;
        }
        
        [Inject]
        private void Constructor(UIStateMachine uiStateMachine)
        {
            _stateMachine = uiStateMachine;
        }

        [Inject]
        private void Constructor(TouchMovement touchMovement)
        {
            _touchMovement = touchMovement;
        }

        private void Awake()
        {
            _touchMovement.OnMatchingCubes += AddToList;
            _testGrid.CounterValueChange += ChangeCounter;
        }

        // private void AddToList(ISelectable first, ISelectable second)
        // {
        //     _colorViews.Add((ColorView)first);
        //     _colorViews.Add((ColorView)second);
        //     if (_colorViews.Count != _touchMovement._colorViews.Count) return;
        //     OnAllCubesMatched?.Invoke();
        //     _touchMovement.EmptySelectables.RemoveRange(0, _touchMovement.EmptySelectables.Count);
        // }

        private void ChangeCounter(int counter)
        {
            _counter = counter;
        }

        private IEnumerator RecreateGridCo()
        {
            yield return new WaitForSeconds(0.1f);
            _testGrid.gameObject.SetActive(true);
        }
        
        private void AddToList(List<ISelectable> selectables)
        {
            switch (_testGrid.GridType)
            {
                case GridType.Default:
                    _emptySelectables.AddRange(selectables);
                    if (_emptySelectables.Count != _touchMovement._colorViews.Count) return;
                    OnAllCubesMatched?.Invoke();
                    break;
                case GridType.DefaultTimer:
                    _emptySelectables.AddRange(selectables);
                    if (_emptySelectables.Count != _touchMovement._colorViews.Count) return;
                    OnAllCubesMatched?.Invoke();
                    break;
                case GridType.EndlessTimer:
                    _emptySelectables.AddRange(selectables);
                    if (_emptySelectables.Count != _touchMovement._colorViews.Count) return;
                    _emptySelectables.RemoveRange(0, _emptySelectables.Count);
                    _testGrid.gameObject.SetActive(false);
                    _stateMachine.TimeValue += 20;
                    _stateMachine.CurrentValue += 20;
                    StartCoroutine(RecreateGridCo());
                    _testGrid.ReCreateGrid();
                    while (_counter < 3)
                    {
                        return;
                    }
                    OnAllCubesMatched?.Invoke();
                    _testGrid.ReCreateGrid();
                    break;
                
            }
            _touchMovement.EmptySelectables.RemoveRange(0, _touchMovement.EmptySelectables.Count);
        }
    }
}