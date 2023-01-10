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
        
        private UIStateMachine _stateMachine;
        
        private TestGrid _testGrid;

        private int _counter;


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
                    _emptySelectables.RemoveRange(0, _emptySelectables.Count);
                    OnAllCubesMatched?.Invoke();
                    break;
                case GridType.DefaultTimer:
                    _emptySelectables.AddRange(selectables);
                    if (_emptySelectables.Count != _touchMovement._colorViews.Count) return;
                    _emptySelectables.RemoveRange(0, _emptySelectables.Count);
                    OnAllCubesMatched?.Invoke();
                    break;
                case GridType.EndlessTimer:
                    _emptySelectables.AddRange(selectables);
                    if (_emptySelectables.Count != _touchMovement._colorViews.Count) return;
                    _emptySelectables.RemoveRange(0, _emptySelectables.Count);
                    _testGrid.gameObject.SetActive(false);
                    _stateMachine.TimeValue += 20;
                    _stateMachine.CurrentValue += 20;
                    _testGrid.ResetCounter();
                    while (_counter < 3)
                    {
                        StartCoroutine(RecreateGridCo());
                        return;
                    }
                    OnAllCubesMatched?.Invoke();
                    _testGrid.ResetCounter();
                    break;
                
            }
            _touchMovement.EmptySelectables.RemoveRange(0, _touchMovement.EmptySelectables.Count);
        }
    }
}