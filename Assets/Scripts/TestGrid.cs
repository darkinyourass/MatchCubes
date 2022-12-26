using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace DefaultNamespace
{
    public class TestGrid : MonoBehaviour
    {
        [SerializeField] private int _height;
        [SerializeField] private int _width;
        [SerializeField] private int _depth;
        [SerializeField] private GameObject _cubePrefab;
        
        [SerializeField] private GridType _gridType;

        public event Action OnGameStarted;

        [Inject]
        private TouchMovement _touchMovement;
        
        [field: SerializeField]
        private ColorView[] Cubes { get; set; }
        
        [Inject]
        private DiContainer _diContainer;

        private GameObject[,,] _grid;

        private void OnEnable()
        {
            CreateGrid();
            Cubes = GetComponentsInChildren<ColorView>();
            _touchMovement._colorViews.AddRange(Cubes);
            OnGameStarted?.Invoke();
        }

        private void OnDisable()
        {
            foreach (var cube in Cubes)
            {
                _touchMovement._colorViews.RemoveRange(0, _touchMovement._colorViews.Count);
                Destroy(cube.gameObject);
            }
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
            _grid = new GameObject[_width, _height, _depth];
            var colors = GenerateColorsPref(_width * _height * _depth);
            var counter = 0;
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    for (var k = 0; k < _depth; k++)
                    {
                        _grid[i, j, k] = _diContainer.InstantiatePrefab(_cubePrefab, new Vector3(i, j, k ),
                            Quaternion.identity, transform);
                        _grid[i, j, k].GetComponent<ISelectable>().SetColor(colors[counter]);
                        counter++;
                    }
                }
            }
        }

        public void SetGridType(GridType gridType)
        {
            _gridType = gridType;
        }
    }
}