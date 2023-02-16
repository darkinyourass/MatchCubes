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
        // private List<ISelectable> _emptySelectables = new ();

        public List<ISelectable> EmptySelectables { get; set; } = new List<ISelectable>();
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
            const float delay = 1.1f;
            yield return new WaitForSeconds(delay);
            _testGrid.gameObject.SetActive(true);
        }

        private IEnumerator SetGridToFalseCo()
        {
            const float delay = 1f;
            yield return new WaitForSeconds(delay);
            _testGrid.gameObject.SetActive(false);
        }
        
        private void AddToList(List<ISelectable> selectables)
        {
            switch (_testGrid.GridType)
            {
                case GridType.Default:
                    EmptySelectables.AddRange(selectables);
                    if (EmptySelectables.Count < _touchMovement.AllCubes.Count) return;
                    EmptySelectables.RemoveRange(0, EmptySelectables.Count);
                    OnAllCubesMatched?.Invoke();
                    break;
                case GridType.DefaultTimer:
                    EmptySelectables.AddRange(selectables);
                    if (EmptySelectables.Count < _touchMovement.AllCubes.Count) return;
                    EmptySelectables.RemoveRange(0, EmptySelectables.Count);
                    OnAllCubesMatched?.Invoke();
                    break;
                case GridType.EndlessTimer:
                    EmptySelectables.AddRange(selectables);
                    if (EmptySelectables.Count < _touchMovement.AllCubes.Count) return;
                    EmptySelectables.RemoveRange(0, EmptySelectables.Count);
                    // _testGrid.gameObject.SetActive(false);
                    StartCoroutine(SetGridToFalseCo());
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
            _touchMovement.EmptyCubes.RemoveRange(0, _touchMovement.EmptyCubes.Count);
        }
    }
}