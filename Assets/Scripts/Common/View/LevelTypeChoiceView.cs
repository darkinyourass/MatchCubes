using System;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Common.View
{
    public class LevelTypeChoiceView : MonoBehaviour, IClickable
    {
        public event Action OnLevelTypeButtonClicked;
        private TestGrid _testGrid;
        private Timer _timer;  
        
        [Inject]
        private void Constructor(Timer timer)
        {
            _timer = timer;
        }

        [Inject]
        private void Constructor(TestGrid testGrid)
        {
            _testGrid = testGrid;
        }

        public void OnPlayButtonClick()
        {
            OnLevelTypeButtonClicked?.Invoke();
            _testGrid.gameObject.SetActive(true);
            switch (_testGrid.GridType)
            {
                case GridType.Default:
                    break;
                case GridType.DefaultTimer:
                    _timer.gameObject.SetActive(true);
                    _timer.IsTimerSet = true;
                    break;
                case GridType.EndlessTimer:
                    _timer.gameObject.SetActive(true);
                    _timer.IsTimerSet = true;
                    break;
            }
        }

        // public void OnDefaultClick()
        // {
        //     OnLevelTypeButtonClicked?.Invoke();
        //     _testGrid.SetGridType(GridType.Default);
        //     _testGrid.gameObject.SetActive(true);
        // }
        //
        // public void OnDefaultTimerClick()
        // {
        //     OnLevelTypeButtonClicked?.Invoke();
        //     _testGrid.SetGridType(GridType.DefaultTimer);
        //     _testGrid.gameObject.SetActive(true);
        //     _timer.gameObject.SetActive(true);
        //     _timer.IsTimerSet = true;
        // }
        //
        // // public void OnEndlessClick()
        // // {
        // //     OnLevelTypeButtonClicked?.Invoke();
        // //     _testGrid.SetGridType(GridType.Endless);
        // //     _testGrid.gameObject.SetActive(true);
        // // }
        //
        // public void OnEndlessTimerClick()
        // {
        //     OnLevelTypeButtonClicked?.Invoke();
        //     _testGrid.SetGridType(GridType.EndlessTimer);
        //     _testGrid.gameObject.SetActive(true);
        //     _timer.gameObject.SetActive(true);
        //     _timer.IsTimerSet = true;
        // }
    }
}