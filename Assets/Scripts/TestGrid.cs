using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using ModestTree;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace DefaultNamespace
{
    public class TestGrid : MonoBehaviour
    {
        [SerializeField] private int _size;

        public int Size
        {
            get => _size;
            set => _size = value;
        }

        [SerializeField] private GameObject _cubePrefab;
        
        public delegate void OnCounterValueChange(int counter);    
        public OnCounterValueChange CounterValueChange;
        
        [field: SerializeField] public GridType GridType { get; private set; }

        public event Action OnGameStarted;

        [Inject]
        private TouchMovement _touchMovement;
        
        [field: SerializeField]
        private ColorView[] Cubes { get; set; }
        [field: SerializeField] private List<ColorView> AllCubes { get; set; }
        
        [Inject]
        private DiContainer _diContainer;

        private GameObject[,,] _grid;

        private const int Counter = 3;
        [SerializeField] private int _counter;

        private void OnEnable()
        {
            CreateGrid();
            Cubes = GetComponentsInChildren<ColorView>();
            AllCubes.AddRange(Cubes);
            _touchMovement._colorViews.AddRange(Cubes);
            OnGameStarted?.Invoke();
        }

        private void OnDisable()
        {
            foreach (var cube in AllCubes)
            {
                Destroy(cube.gameObject);
            }
            AllCubes.RemoveRange(0, AllCubes.Count);
            _touchMovement._colorViews.RemoveRange(0, _touchMovement._colorViews.Count);
        }

        public void ReCreateGrid()
        {
            while (_counter < Counter)
            {
                _counter++;
                CounterValueChange?.Invoke(_counter);
                return;
            }
            _counter = 0;
            CounterValueChange?.Invoke(_counter);
        }
        
        private int[] GenerateColorsPref(int amount) {
            var random = new Random();
            var colors = new List<int>();
            var color = 0;
            for (var j = 0; j < 4; j++) {
                for (var i = 0; i < amount / 4; i++) {
                    colors.Add(color);
                }
                color++;
            }
            return colors.OrderBy(item => random.Next()).ToArray();
        }
        
        private void CreateGrid()
        {
            _grid = new GameObject[_size, _size, _size];
            var colors = GenerateColorsPref(_size * _size * _size);
            var counter = 0;
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    for (var k = 0; k < _size; k++)
                    {
                        _grid[i, j, k] = _diContainer.InstantiatePrefab(_cubePrefab, new Vector3(i, j, k ),
                            Quaternion.identity, transform);
                        if (_size != 2)
                        {
                            if ((i == _size / 2 || i == (_size / 2) - 1) && (j == _size / 2 || j == (_size / 2) - 1) &&
                                (k == _size / 2 || k == (_size / 2) - 1))
                            {
                                _grid[i, j, k].GetComponent<ISelectable>().SetColor((int)ColorType.White);
                            }
                            else
                            {
                                _grid[i, j, k].GetComponent<ISelectable>().SetColor(colors[counter]);
                                counter++;
                            }
                        }
                        else
                        {
                            _grid[i, j, k].GetComponent<ISelectable>().SetColor(colors[counter]);
                            counter++;
                        }
                    }
                }
            }
        }

        public void SetGridType(GridType gridType)
        {
            GridType = gridType;
        }
    }
}