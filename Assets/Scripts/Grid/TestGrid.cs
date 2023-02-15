using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using Common.View.Tutorial;
using UI;
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
        
        [SerializeField] private int _height;
        [SerializeField] private int _width;
        [SerializeField] private int _depth;
        
        [SerializeField] private GameObject _cubePrefab;
        
        public bool isTutorialFinished;

        public static bool IsFirstClicked;
        public static bool IsSecondClicked;
        public static bool IsThirdClicked;
        public static bool IsFourthClicked;
        
        public bool IsCameraRotatingAvailable { get; set; }
        // public static bool IsFifthClicked;

        public delegate void OnCounterValueChange(int counter);    
        public OnCounterValueChange CounterValueChange;
        public event Action OnGameStarted;
        
        [field: Header("Set level type")]
        [field: SerializeField] public GridType GridType { get; set; }

        [field: Header("All cubes")]
        [field: SerializeField] private Cube[] Cubes { get; set; }
        [field: SerializeField] private List<Cube> AllCubes { get; } = new();
        
        public int Size => _size;

        private GameObject[,,] _grid;

        [SerializeField] private Tutorial _tutorial;
        
        [Header("Counter for endless level")]
        [SerializeField] private int _counter;

        private const int Counter = 3;

        [Header("Spawn Delay")] 
        [SerializeField] private float _spawnDelay;


        [SerializeField] private TutorialPanel _tutorialPanel;

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
                Cubes = GetComponentsInChildren<Cube>();
                AllCubes.AddRange(Cubes);
                _touchMovement.AllCubes.AddRange(Cubes);
                OnGameStarted?.Invoke();
            }
            else
            {
                IsCameraRotatingAvailable = true;
                StartCoroutine(CreateGrid());
                // CreateGrid();
            }
            
            // StartCoroutine(SpawnAnimationCo());
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
            _touchMovement.AllCubes.RemoveRange(0, _touchMovement.AllCubes.Count);
        }

        // private void Update()
        // {
        //     Debug.Log($"AllCubes count - {AllCubes.Count}");
        //     Debug.Log($"ColorViews count - {_touchMovement._colorViews.Count}");
        // }

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

        private IEnumerator CreateGrid()
        {
            _grid = new GameObject[_width, _height, _depth];
            var colors = GenerateColorsPref(_width * _height * _depth);
            var counter = 0;
            var randomIndex = UnityEngine.Random.Range(0, 8);
            var currentCenterCube = 0;
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    for (var k = 0; k < _depth; k++)
                    {
                        
                        _grid[i, j, k] = _diContainer.InstantiatePrefab(_cubePrefab, new Vector3(i, j, k ),
                            Quaternion.identity, transform);
                        _grid[i, j, k].GetComponent<ISelectable>()
                            .Animator.SetBool(_grid[i, j, k].GetComponent<ISelectable>().SpawningAnimationHash, true);
                        
                        

                        AllCubes.Add((Cube)_grid[i, j, k].GetComponent<ISelectable>());
                        _touchMovement.AllCubes.Add((Cube)_grid[i, j, k].GetComponent<ISelectable>());
                        
                        
                        if (_size != 2)
                        {
                            if ((i == _width / 2 || i == (_width / 2) - 1) && (j == _height / 2 || j == (_height / 2) - 1) &&
                                (k == _depth / 2 || k == (_depth / 2) - 1))
                            {
                                if (randomIndex == currentCenterCube)
                                {
                                    _grid[i, j, k].GetComponent<ISelectable>().SetColor((int) ColorType.White);
                                }
                                else
                                {
                                    _grid[i, j, k].GetComponent<ISelectable>().SetColor(colors[counter]);
                                    counter++;
                                }

                                currentCenterCube++;
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


            yield return new WaitForSeconds(_spawnDelay);

            foreach (var cube in AllCubes)
            {
                cube.Animator.SetBool(cube.SpawningAnimationHash, false);
            }
            OnGameStarted?.Invoke();
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
            _tutorial.SetTutorialText("TAP AND HOLD TO SELECT CUBE");
            _grid[0, 1, 1].GetComponent<ISelectable>().SetColor((int)ColorType.Green);
            _grid[1, 0, 0].GetComponent<ISelectable>().SetColor((int)ColorType.Green);
            _grid[1, 0, 1].GetComponent<ISelectable>().SetColor((int)ColorType.White);
            _grid[1, 1, 0].GetComponent<ISelectable>().SetColor((int)ColorType.Blue);
            _grid[1, 1, 1].GetComponent<ISelectable>().SetColor((int)ColorType.Yellow);
        }
        
        private void ClickFirstCube()
        {
            _tutorial.SetHandImagePosition(_grid[1, 1, 0].transform.position + new Vector3(0.5f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("MOVE YOUR FINGER TO THE SAME CUBE TO SELECT AND MERGE");
            IsFirstClicked = true;
        }

        private void ClickSecondCube()
        {
            _tutorial.SetHandImagePosition(_grid[1, 0, 0].transform.position + new Vector3(0.5f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("SELECT CUBE");
            IsSecondClicked = true;
        }

        private void ClickThird()
        {
            _tutorial.SetHandImagePosition(_grid[1, 1, 0].transform.position + new Vector3(0.5f, -0.5f, -0.7f));
            _tutorial.SetTutorialText("MOVE YOUR FINGER TO THE EMPTY CUBE TO MOVE");
            IsThirdClicked = true;
        }

        private void ClickFourth()
        {
            IsFourthClicked = true;
            _tutorial.gameObject.SetActive(false);


            IEnumerator SetPanel()
            {
                IsCameraRotatingAvailable = true;
                yield return new WaitForSeconds(0.5f);
                _tutorialPanel.gameObject.SetActive(true);
            }

            StartCoroutine(SetPanel());
        }
    }
}