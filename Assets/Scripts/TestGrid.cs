using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using UI;
using UnityEditor;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace DefaultNamespace
{
    public class TestGrid : MonoBehaviour
    {
        [Inject] private TouchMovement _touchMovement;
        
        [Inject] private DiContainer _diContainer;
        
        [Header("Size")]
        [SerializeField] private int _size;
        [SerializeField] private GameObject _cubePrefab;
        
        public bool isTutorialFinished;

        public static bool IsFirstClicked;
        public static bool IsSecondClicked;
        public static bool IsThirdClicked;
        public static bool IsFourthClicked;

        public delegate void OnCounterValueChange(int counter);    
        public OnCounterValueChange CounterValueChange;
        public event Action OnGameStarted;
        
        [field: Header("Set level type")]
        [field: SerializeField] public GridType GridType { get; set; }

        [field: Header("All cubes")]
        [field: SerializeField] private ColorView[] Cubes { get; set; }
        [field: SerializeField] private List<ColorView> AllCubes { get; } = new();
        
        public int Size => _size;

        private GameObject[,,] _grid;

        [SerializeField] private Tutorial _tutorial;
        
        [Header("Counter for endless level")]
        [SerializeField] private int _counter;

        private const int Counter = 3;

        private void OnEnable()
        {
            int boolValue = PlayerPrefs.GetInt("Tutorial", 0);
            isTutorialFinished = boolValue != 0;
            if (!isTutorialFinished)
            {
                _touchMovement.OnTutorialCubeClick += ClickFirstCube;
                _touchMovement.OnTutorialSecondCubeClick += ClickSecondCube;
                _touchMovement.OnTutorialThirdClick += ClickThird;
                _touchMovement.OnTutorialFourthClick += ClickFourth;
                CreateTutorialGrid();
            }
            else
            {
                CreateGrid();
            }

            Cubes = GetComponentsInChildren<ColorView>();
            AllCubes.AddRange(Cubes);
            _touchMovement._colorViews.AddRange(Cubes);
            OnGameStarted?.Invoke();
        }

        private void OnDisable()
        {
            _touchMovement.OnTutorialCubeClick -= ClickFirstCube;
            _touchMovement.OnTutorialSecondCubeClick -= ClickSecondCube;
            _touchMovement.OnTutorialThirdClick -= ClickThird;
            _touchMovement.OnTutorialFourthClick -= ClickFourth;
            foreach (var cube in AllCubes)
            {
                Destroy(cube.gameObject);
            }
            AllCubes.RemoveRange(0, AllCubes.Count);
            _touchMovement._colorViews.RemoveRange(0, _touchMovement._colorViews.Count);
        }

        public void UpdateValue(bool value)
        {
            isTutorialFinished = value;
            PlayerPrefs.SetInt("Tutorial", 1);
            PlayerPrefs.Save();
        }

        public void ResetCounter()
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

        private void CreateTutorialGrid()
        {
            _grid = new GameObject[_size, _size, _size];
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    for (var k = 0; k < _size; k++)
                    {
                        _grid[i, j, k] = _diContainer.InstantiatePrefab(_cubePrefab, new Vector3(i, j, k ),
                            Quaternion.identity, transform);
                    }
                }
            }
            
            _grid[0, 0, 0].GetComponent<ISelectable>().SetColor((int)ColorType.Red);
            _grid[0, 0, 1].GetComponent<ISelectable>().SetColor((int)ColorType.Red);
            _grid[0, 1, 0].GetComponent<ISelectable>().SetColor((int)ColorType.Blue);
            _tutorial.SetHandImagePosition(_grid[0, 1, 0].transform.position + new Vector3(0.6f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("TAP ON CUBE");
            _grid[0, 1, 1].GetComponent<ISelectable>().SetColor((int)ColorType.Green);
            _grid[1, 0, 0].GetComponent<ISelectable>().SetColor((int)ColorType.Green);
            _grid[1, 0, 1].GetComponent<ISelectable>().SetColor((int)ColorType.Yellow);
            _grid[1, 1, 0].GetComponent<ISelectable>().SetColor((int)ColorType.Blue);
            _grid[1, 1, 1].GetComponent<ISelectable>().SetColor((int)ColorType.Yellow);
        }
        
        private void ClickFirstCube()
        {
            _tutorial.SetHandImagePosition(_grid[1, 1, 0].transform.position + new Vector3(0.5f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("TAP TO MERGE");
            IsFirstClicked = true;
        }

        private void ClickSecondCube()
        {
            _tutorial.SetHandImagePosition(_grid[1, 0, 0].transform.position + new Vector3(0.5f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("TAP ON CUBE");
            IsSecondClicked = true;
        }

        private void ClickThird()
        {
            _tutorial.SetHandImagePosition(_grid[1, 1, 0].transform.position + new Vector3(0.5f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("TAP TO MOVE");
            IsThirdClicked = true;
        }

        private void ClickFourth()
        {
            IsFourthClicked = true;
            _tutorial.gameObject.SetActive(false);
        }
    }
}