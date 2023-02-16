using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using DefaultNamespace;
using Game.Camera;
using GameInput;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;


public class TouchMovement : MonoBehaviour
{
    private TestGrid _testGrid;

    private UIStateMachine _uiStateMachine;

    private CoinsHolder _coinsHolder;

    [SerializeField] private float _movementDuration;
    
    [Header("Audio")]
    [SerializeField] private AudioClip _matchAudioClip;
    [SerializeField] private AudioClip _wrongAudioClip;

    private Touch _touch;

    [SerializeField] private LayerMask _layerMask;
    
    private int _selectableLayerMask;
    private int _ignoreLayerMask;

    public event Action<List<ISelectable>> OnMatchingCubes;
    public event Action OnMakeMove;

    private readonly List<ISelectable> _emptySelectablesNearFirstSelectable = new ();
    public List<ISelectable> AllCubes { get; set; } = new();

    private readonly List<ISelectable> _anotherList = new ();
    
    private readonly List<ISelectable> _mergedCubes = new ();
    
    private bool IsMoving { get; set; }
    
    // private bool IsWhiteCube { get; set; }
    
    public List<ISelectable> EmptyCubes { get; set; } = new();

    private List<ISelectable> SelectedCubes { get; } = new ();

    private List<ISelectable> SecondDirectionSelectables { get; } = new ();

    private TouchInput _touchInput;
    private Ray _ray;

    private ISelectable _lastSelectable;
    private ISelectable _firstSelectable;
    private ISelectable _previousSelectable;
    private ISelectable _secondSelectable;
    private ISelectable _selection;
    
    private Vector3 _lastSelectablePosition = Vector3.zero;
    private Vector3 _firstSelectablePosition = Vector3.zero;

    private Camera _camera;

    private ThrowingObject _throwingObject;
    private ObjectRotation _objectRotation;
    
    public event Action OnTutorialCubeClick;
    public event Action OnTutorialSecondCubeClick;
    public event Action OnTutorialThirdClick;
    public event Action OnTutorialFourthClick;

    public bool IsSelectingCubes { get; set; }
    
    [Inject]
    private void Constructor(TestGrid testGrid, UIStateMachine stateMachine, CoinsHolder coinsHolder)
    {
        _testGrid = testGrid;
        _uiStateMachine = stateMachine;
        _coinsHolder = coinsHolder;
    }

    private void Start()
    {
        _objectRotation = FindObjectOfType<ObjectRotation>();
        _throwingObject = FindObjectOfType<ThrowingObject>();
        _camera = FindObjectOfType<Camera>();
        _selectableLayerMask = LayerMask.NameToLayer("Selectables");
        _ignoreLayerMask = LayerMask.NameToLayer("Ignore Raycast");
        _touchInput = GetComponent<TouchInput>();
        _touchInput.OnTouchOrMouseUp += DeselectCubes;
        _touchInput.OnMouseDownOverCube += SelectCubes;
        _testGrid.OnGameStarted += StartGame;
    }
    

    private void Update()
    {
        SetLayerMask();
        // Debug.Log($"Last selectable color - {_lastSelectable.ColorType}");
        // Debug.Log($"EmptyCount - {EmptySelectables.Count}");
        // Debug.Log($"First selectable color - {_firstSelectable.ColorType}");
        // Debug.Log($"SelectablesCount - {SelectedCubes.Count}");
        // Debug.Log($"SecondSelectablesCount - {SecondDirectionSelectables.Count}");
    }

    private void OnDisable()
    {
        _testGrid.OnGameStarted -= StartGame;
        _touchInput.OnTouchOrMouseUp -= DeselectCubes;
        _touchInput.OnMouseDownOverCube -= SelectCubes;
    }

    private void StartGame()
    {
        
    }

    private bool CheckForDirection()
    {
        var heading = _selection.ColorTypeTransform.position - _firstSelectablePosition;
        var distance = heading.magnitude;
        var direction = heading / distance;
        
        if (direction.x != 1 && direction.y != 1 && direction.z != 1 && direction.x != -1 && direction.y != -1 &&
            direction.z != -1)
        {
            // AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
            return false;
        }

        return true;
    }

    private void MergeSelectableCubes()
    {
        if (CheckForDirection())
        {
            if (CheckForMoving())
            {
                foreach (var cube in SelectedCubes.Where(cube => cube != _firstSelectable))
                {
                    switch (cube.MeshRenderer.enabled)
                    {
                        case false:
                            continue;
                        case true when cube != _firstSelectable:
                            AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
                            return;
                    }
                }

                _lastSelectable.ColorTypeTransform.position = _firstSelectablePosition;
                StartCoroutine(MoveCubeCo(_lastSelectablePosition));
                return;
            }

            if (!CheckForMoving())
            {
                foreach (var cube in SelectedCubes)
                {
                    if (SelectedCubes.Count <= 1)
                    {
                        return;
                    }
                    if (cube == _firstSelectable)
                    {
                        continue;
                    }

                    if (cube.MeshRenderer.enabled && cube.ColorType == _firstSelectable.ColorType)
                    {
                        continue;
                    }
                    
                    if (cube.MeshRenderer.enabled == false ||
                        cube.ColorType != _firstSelectable.ColorType)
                    {
                        AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
                        return;
                    }
                }
                
                foreach (var selectable in SelectedCubes)
                {
                    selectable.MeshRenderer.enabled = false;
                    selectable.ParticleSystem.Play();
                    selectable.EmptyCubeLineRenderer.enabled = true;
                }
                OnMatchingCubes?.Invoke(SelectedCubes);
                AudioManager.Instance.PlayAudioClip(_matchAudioClip);
                OnMakeMove?.Invoke();
                _secondSelectable = null;
                CheckForMatchingSelectablesCount();
                EmptyCubes.AddRange(SelectedCubes);
            }
        }
    }

    private bool CheckForMoving()
    {
        if (_lastSelectable != null)
        {
            return _lastSelectable.MeshRenderer.enabled == false;
        }

        return true;
    }

    // private void SelectCubes()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //     {
    //         return;
    //     }
    //     
    //     _ray = _camera.ScreenPointToRay(Input.mousePosition);
    //
    //     if (Physics.Raycast(_ray, out var hit, _layerMask))
    //     {
    //         _selection = hit.transform.GetComponent<ISelectable>();
    //         if (_selection != null)
    //         {
    //             if (_firstSelectable == null)
    //             {
    //                 _firstSelectable = _selection;
    //                 _firstSelectablePosition = _firstSelectable.ColorTypeTransform.position;
    //                 SetEmptyCubes();
    //                 CheckForNearestSelectables();
    //             }
    //
    //             // foreach (var cube in SecondDirectionSelectables)
    //             // {
    //             //     if (_firstSelectable != null)
    //             //     {
    //             //         if (_secondSelectable != null)
    //             //         {
    //             //             if (_secondSelectable != selectable 
    //             //                 && selectable != _firstSelectable 
    //             //                 && selectable == cube 
    //             //                 && SecondDirectionSelectables.Contains(selectable))
    //             //             {
    //             //                 _secondSelectable.SelectDeselect();
    //             //                 _previousSelectable = _secondSelectable;
    //             //                 _secondSelectable.SelectedCubeLineRenderer.enabled = false;
    //             //                 _secondSelectable = selectable;
    //             //                 // Debug.Log($"Second choice {_secondSelectable.ColorType}");
    //             //             }
    //             //         }
    //             //     
    //             //         if (_secondSelectable == null && selectable != _firstSelectable && selectable == cube)
    //             //         {
    //             //             // _previousSelectable = selectable;
    //             //             _secondSelectable = selectable;
    //             //             // Debug.Log($"First choice {_secondSelectable.ColorType}");
    //             //         }
    //             //     }
    //             // }
    //             //
    //             // if (_secondSelectable != null)
    //             // {
    //             //     Debug.Log(_secondSelectable.ColorType);
    //             // }
    //             // if (_firstSelectable.ColorType == ColorType.White)
    //             // {
    //             //     CheckForWhiteCube();
    //             //     return;
    //             // }
    //             
    //             if (_selection.IsSelected)
    //             {
    //                 return;
    //             }
    //
    //             foreach (var cube in SecondDirectionSelectables)
    //             {
    //                 if (cube == _selection)
    //                 {
    //                     CheckForCubeSelectionDirection();
    //                 }
    //             }
    //
    //             _lastSelectable = _selection;
    //             _lastSelectablePosition = _lastSelectable.ColorTypeTransform.position;
    //             SelectedCubes.Add(_selection);
    //             _selection.SelectDeselect();
    //             _selection.SelectedCubeLineRenderer.enabled = true;
    //             
    //             Debug.Log(SelectedCubes.Count);
    //             
    //             // if (_previousSelectable != null)
    //             // {
    //             //     if (_secondSelectable != _previousSelectable)
    //             //     {
    //             //         foreach (var cube in SelectedCubes)
    //             //         {
    //             //             if (cube == _previousSelectable)
    //             //             {
    //             //                 SelectedCubes.Remove(cube);
    //             //                 return;
    //             //             }
    //             //         }
    //             //     }
    //             // }
    //         }
    //     }
    // }


    private bool IsGoingBackwards(Vector3 lastPosition, Vector3 currentPosition, Vector3 firstPosition)
    {
        var headingCurrentFirst = currentPosition - firstPosition;
        var distanceCurrentFirst = headingCurrentFirst.magnitude;
        var directionCurrentFirst = headingCurrentFirst / distanceCurrentFirst;
        
        var headingCurrentLast = currentPosition - lastPosition;
        var distanceCurrentLast = headingCurrentLast.magnitude;
        var directionCurrentLast = headingCurrentLast / distanceCurrentLast;

        return directionCurrentFirst != directionCurrentLast;
    }
    
    private void SelectCubes()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (_objectRotation.IsRotating || _objectRotation.IsZooming || Input.touchCount != 1)
        {
            return;
        }
        
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
    
        if (Physics.Raycast(_ray, out var hit, _layerMask))
        {
            _selection = hit.transform.GetComponent<ISelectable>();
            if (_selection != null)
            {
                IsSelectingCubes = true;
                if (_firstSelectable == null)
                {
                    _firstSelectable = _selection;
                    _firstSelectablePosition = _firstSelectable.ColorTypeTransform.position;
                    SelectedCubes.Add(_selection);
                    _selection.SelectDeselect();
                    _selection.SelectedCubeLineRenderer.enabled = true;
                    _lastSelectable = _firstSelectable;
                    _lastSelectablePosition = _firstSelectablePosition;
                    SetEmptyCubes();
                    CheckForNearestSelectables();
                    return;
                }

                if (_firstSelectable.ColorType == ColorType.White)
                {
                    // if (!TestGrid.IsFifthClicked)
                    // {
                    //     OnTutorialFifthCubeClick?.Invoke();
                    // }

                    CheckForWhiteCube();
                    return;
                }
                
                switch (_testGrid.isTutorialFinished)
                {
                    case false when !TestGrid.IsFirstClicked &&
                                    _firstSelectable.ColorTypeTransform.position == new Vector3(0, 1, 0):
                        OnTutorialCubeClick?.Invoke();
                        break;
                    case false when !TestGrid.IsThirdClicked &&
                                    _firstSelectable.ColorTypeTransform.position == new Vector3(1, 0, 0):
                        OnTutorialThirdClick?.Invoke();
                        break;
                }

                if (_secondSelectable == null && _firstSelectable != null && _selection != _firstSelectable)
                {
                    if (CheckForDirection() && Vector3.Distance(_selection.ColorTypeTransform.position, _firstSelectablePosition) <= 1f)
                    {
                        _secondSelectable = _selection;
                        SelectedCubes.Add(_selection);
                        _selection.SelectDeselect();
                        _selection.SelectedCubeLineRenderer.enabled = true;
                        _lastSelectable = _secondSelectable;
                        _lastSelectablePosition = _secondSelectable.ColorTypeTransform.position;
                        return;
                    }
                }

                if (_secondSelectable != null && _firstSelectable != _secondSelectable)
                {
                    var count = 0;
                    foreach (var cube in SecondDirectionSelectables)
                    {
                        if (_selection == cube && _secondSelectable != cube)
                        {
                            goto FoundSecondCube;
                        }
                        if (_selection != cube)
                        {
                            count++;
                            if (count == SecondDirectionSelectables.Count)
                            {
                                goto Continue;
                            }
                        }
                    }
                    
                    FoundSecondCube:
                    if (_secondSelectable != _selection)
                    {
                        foreach (var cube in SecondDirectionSelectables.Where(cube => SelectedCubes.Contains(cube)))
                        {
                            cube.SelectDeselect();
                            cube.SelectedCubeLineRenderer.enabled = false;
                            SelectedCubes.Remove(cube);
                        }

                        var currentSelectables = SelectedCubes
                            .Where(cube => cube != _firstSelectable && cube != _secondSelectable).ToList();

                        foreach (var cube in currentSelectables.Where(cube => SelectedCubes.Contains(cube)))
                        {
                            cube.SelectedCubeLineRenderer.enabled = false;
                            cube.SelectDeselect();
                            SelectedCubes.Remove(cube);
                        }

                        _secondSelectable = _selection;
                        SelectedCubes.Add(_selection);
                        _selection.SelectDeselect();
                        _selection.SelectedCubeLineRenderer.enabled = true;
                        _lastSelectable = _secondSelectable;
                        _lastSelectablePosition = _secondSelectable.ColorTypeTransform.position;
                        return;
                    }
                }
                
                Continue:
                
                if (_firstSelectable != _selection)
                {
                    var headingFirstLast = _selection.ColorTypeTransform.position - _firstSelectablePosition;
                    var distanceFirstLast = headingFirstLast.magnitude;
                    var directionFirstLast = headingFirstLast / distanceFirstLast;

                    if (directionFirstLast.x != 1 && directionFirstLast.y != 1 && directionFirstLast.z != 1
                        && directionFirstLast.x != -1 && directionFirstLast.y != -1 && directionFirstLast.z != -1
                        || Vector3.Distance(_selection.ColorTypeTransform.position, _lastSelectablePosition) > 1f)
                    {
                        return;
                    }
                }
                
                if (IsGoingBackwards(_lastSelectablePosition,
                        _selection.ColorTypeTransform.position,
                        _firstSelectablePosition))
                {
                    if (_selection == _lastSelectable)
                    {
                        return;
                    }
                    _lastSelectable.SelectDeselect();
                    _lastSelectable.SelectedCubeLineRenderer.enabled = false;
                    SelectedCubes.Remove(_lastSelectable);
                    _lastSelectable = _selection;
                    _lastSelectablePosition = _lastSelectable.ColorTypeTransform.position;
                }

                else if (!IsGoingBackwards(_lastSelectablePosition,
                             _selection.ColorTypeTransform.position,
                             _firstSelectablePosition))
                {
                    if (_selection.IsSelected)
                    {
                        return;
                    }
                    _lastSelectable = _selection;
                    _lastSelectablePosition = _lastSelectable.ColorTypeTransform.position;
                    SelectedCubes.Add(_selection);
                    _selection.SelectDeselect();
                    _selection.SelectedCubeLineRenderer.enabled = true;
                }
                
            }
        }
    }

    private void DeselectCubes()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (_objectRotation.IsRotating)
        {
            return;
        }

        if (_firstSelectable == _lastSelectable)
        {
            foreach (var cube in SelectedCubes)
            {
                cube.SelectDeselect();
                cube.SelectedCubeLineRenderer.enabled = false;
            }
            SelectedCubes.RemoveRange(0, SelectedCubes.Count);
            _firstSelectable = null;
            _lastSelectable = null;
            IsSelectingCubes = false;
            return;
        }
        
        if (_firstSelectable.ColorType == ColorType.White)
        {
            CheckForWhiteCube();
            _firstSelectable.SelectedCubeLineRenderer.enabled = false;
            return;
        }
        SecondDirectionSelectables.RemoveRange(0, SecondDirectionSelectables.Count);
        MergeSelectableCubes();
        
        switch (_testGrid.isTutorialFinished)
        {
            case false when !TestGrid.IsSecondClicked &&
                            _lastSelectable.ColorTypeTransform.position == new Vector3(1, 1, 0):
                OnTutorialSecondCubeClick?.Invoke();
                break;
            case false when !TestGrid.IsFourthClicked &&
                            _lastSelectablePosition == new Vector3(1, 1, 0):
                OnTutorialFourthClick?.Invoke();
                break;
        }
        
        if (!IsMoving)
        {
            foreach (var cube in SelectedCubes)
            {
                cube.SelectDeselect();
                cube.SelectedCubeLineRenderer.enabled = false;
            }
            SelectedCubes.RemoveRange(0, SelectedCubes.Count);
            _firstSelectablePosition = Vector3.zero;
            _firstSelectable = null;
            _lastSelectablePosition = Vector3.zero;
            _lastSelectable = null;
            _emptySelectablesNearFirstSelectable.RemoveRange(0, _emptySelectablesNearFirstSelectable.Count);
        }
        IsSelectingCubes = false;
    }

    private void CheckForWhiteCube()
    {
        // OnMakeMove?.Invoke();
        // IsWhiteCube = true;
        foreach (var cube in AllCubes)
        {
            cube.EmptyCubeLineRenderer.enabled = false;
            if (cube.MeshRenderer.enabled)
            {
                EmptyCubes.Add(cube);
                _anotherList.Add(cube);
                cube.MeshRenderer.enabled = false;
                cube.ParticleSystem.Play();
            }
        }
        OnMatchingCubes?.Invoke(_anotherList);
        _anotherList.RemoveRange(0, _anotherList.Count);
    }

    private IEnumerator MoveCubeCo(Vector3 secondPosition)
    {
        float timeElapsed = 0;
        var startPosition = _firstSelectable.ColorTypeTransform.position;
        while (timeElapsed < _movementDuration)
        {
            IsMoving = true;
            _firstSelectable.ColorTypeTransform.position = Vector3.Lerp(startPosition, secondPosition, timeElapsed / _movementDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        OnMakeMove?.Invoke();
        _firstSelectable.ColorTypeTransform.position = secondPosition;
        IsMoving = false;
        foreach (var cube in SelectedCubes)
        {
            cube.SelectDeselect();
            cube.SelectedCubeLineRenderer.enabled = false;
        }
        SelectedCubes.RemoveRange(0, SelectedCubes.Count);
        _firstSelectable = null;
        _secondSelectable = null;
        _lastSelectable = null;
        _emptySelectablesNearFirstSelectable.RemoveRange(0, _emptySelectablesNearFirstSelectable.Count);
    }

    private void SetEmptyCubes()
    {
        foreach (var cube in AllCubes)
        {
            var position = cube.ColorTypeTransform.position;
            var heading = position - _firstSelectablePosition;
            var distance = heading.magnitude;
            var direction = heading / distance;

            if (direction.x == 1 || direction.y == 1 || direction.z == 1
                || direction.x == -1 || direction.y == -1 || direction.z == -1)
            {
                _emptySelectablesNearFirstSelectable.Add(cube);
            }
        }
    }

    private void CheckForNearestSelectables()
    {
        foreach (var cube in _emptySelectablesNearFirstSelectable)
        {
            var position = cube.ColorTypeTransform.position;
            var heading = position - _firstSelectablePosition;
            var distance = heading.magnitude;
            var direction = heading / distance;

            if ((direction.x == 1 || direction.y == 1 || direction.z == 1
                 || direction.x == -1 || direction.y == -1 || direction.z == -1) 
                && Vector3.Distance(_firstSelectablePosition, cube.ColorTypeTransform.position) <= 1f)
            {
                if (cube != _firstSelectable)
                {
                    SecondDirectionSelectables.Add(cube);
                }
            }
        }
    }

    private void SetLayerMask()
    {
        if (_firstSelectable == null)
        {
            foreach (var emptySelectable in EmptyCubes.Where(emptySelectable => emptySelectable.ColorTypeTransform != null))
            {
                emptySelectable.ColorTypeTransform.gameObject.layer = _ignoreLayerMask;
            }
        }
        
        if (_firstSelectable != null)
        {
            foreach (var emptySelectable in _emptySelectablesNearFirstSelectable)
            {
                if (emptySelectable.ColorTypeTransform != null)
                {
                    emptySelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                }
            }
        }
        
    }

    private void CheckForMatchingSelectablesCount()
    {
        switch (SelectedCubes.Count)
        {
            case 2:
                _coinsHolder.UpdateValue(2);
                _uiStateMachine.PopTextView.gameObject.SetActive(true);
                _uiStateMachine.PopTextView.SetTextValue("NICE");
                break;
            case 3:
                _coinsHolder.UpdateValue(4);
                _uiStateMachine.PopTextView.gameObject.SetActive(true);
                _uiStateMachine.PopTextView.SetTextValue("COOL");
                break;
            case 4:
                _coinsHolder.UpdateValue(8);
                _uiStateMachine.PopTextView.gameObject.SetActive(true);
                _uiStateMachine.PopTextView.SetTextValue("RADICAL");
                FindAndMergeCubes(1);
                break;
            case 5:
                _coinsHolder.UpdateValue(12);
                _uiStateMachine.PopTextView.gameObject.SetActive(true);
                _uiStateMachine.PopTextView.SetTextValue("AWESOME");
                FindAndMergeCubes(3);
                break;
            case 6:
                _coinsHolder.UpdateValue(20);
                _uiStateMachine.PopTextView.gameObject.SetActive(true);
                _uiStateMachine.PopTextView.SetTextValue("PERFECT");
                FindAndMergeCubes(5);
                break;
        }
    }
    
    private void FindAndMergeCubes(int pairs)
    {
        var cubesInPairs = new List<ISelectable>();
        AllCubes = AllCubes.OrderBy(x => Random.value).ToList();
        var distinctColors = AllCubes.Select(x => x.ColorType).ToList();

        for (var i = 0; i < pairs; i++)
        {
            var color = distinctColors[Random.Range(0, distinctColors.Count)];
            
            var cubesWithColor = AllCubes.Where(x =>
                x.ColorType == color && x.MeshRenderer.enabled && x.ColorType != ColorType.White &&
                !SelectedCubes.Contains(x)).ToList();

            if (color == ColorType.White)
            {
                i--;
                continue;
            }

            if (cubesWithColor.Count < 2) continue;
            cubesInPairs.Add(cubesWithColor[0]);
            cubesInPairs.Add(cubesWithColor[1]);
        }
        MergeCubes(cubesInPairs);
    }

    private void MergeCubes(List<ISelectable> cubes)
    {                 
        _mergedCubes.AddRange(cubes);
        OnMatchingCubes?.Invoke(_mergedCubes);
        
        EmptyCubes.AddRange(cubes);
        _throwingObject.Throw(_mergedCubes, cubes);
    }
}
