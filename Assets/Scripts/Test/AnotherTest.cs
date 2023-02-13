using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using DefaultNamespace;
using GameInput;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;

namespace Test
{
    public class AnotherTest : MonoBehaviour
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
    // public event Action OnTutorialCubeClick;
    // public event Action OnTutorialSecondCubeClick;
    // public event Action OnTutorialThirdClick;
    // public event Action OnTutorialFourthClick;

    private readonly List<ISelectable> _emptySelectablesNearFirstSelectable = new ();
    public List<ISelectable> AllCubes { get; set; } = new();

    private readonly List<ISelectable> _anotherList = new ();
    
    private readonly List<ISelectable> _mergedCubes = new ();
    
    private bool IsMoving { get; set; }
    
    public List<ISelectable> EmptySelectables { get; set; } = new();

    private List<ISelectable> SelectedCubes { get; } = new ();

    private TouchInput _touchInput;

    private ISelectable _lastSelectable;
    private ISelectable _firstSelectable;
    private ISelectable _selection;
    
    private Vector3 _lastSelectablePosition;
    private Vector3 _firstSelectablePosition;

    private Camera _camera;

    private ThrowingObject _throwingObject;
    
    [Inject]
    private void Constructor(TestGrid testGrid, UIStateMachine stateMachine, CoinsHolder coinsHolder)
    {
        _testGrid = testGrid;
        _uiStateMachine = stateMachine;
        _coinsHolder = coinsHolder;
    }

    private void Start()
    {
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
        // Debug.Log($"First selectable color - {_firstSelectable}");
        // Debug.Log($"SelectablesCount - {SelectedCubes.Count}");
        SetLayerMask();
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
        var heading = _lastSelectablePosition - _firstSelectablePosition;
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
            foreach (var cube in SelectedCubes)
            {
                if (CheckForEmptySelectables())
                {
                    switch (cube.MeshRenderer.enabled)
                    {
                        case true when cube != _firstSelectable:
                            AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
                            return;

                        case false:
                            continue;
                    }

                    _lastSelectable.ColorTypeTransform.position = _firstSelectablePosition;
                    StartCoroutine(MoveCubeCo(_lastSelectablePosition));
                    return;
                }

                if (CheckForEmptySelectables()) continue;
                if (cube.ColorType != _firstSelectable.ColorType && cube.MeshRenderer.enabled || cube.MeshRenderer.enabled == false)
                {
                    AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
                }

                if (cube.ColorType == _firstSelectable.ColorType && cube.MeshRenderer.enabled)
                {
                    continue;
                }

                return;
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
            CheckForMatchingSelectablesCount();
            EmptySelectables.AddRange(SelectedCubes);
        }
    }

    private bool CheckForEmptySelectables()
    {
        return _lastSelectable.MeshRenderer.enabled == false;
    }

    private void SelectCubes()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, _layerMask))
        {
            var selectable = hit.transform.GetComponent<ISelectable>();
             if (selectable != null)
            {
                if (_firstSelectable == null)
                {
                    
                    _firstSelectable = selectable;
                    _firstSelectablePosition = _firstSelectable.ColorTypeTransform.position;
                    SetEmptyCubes();
                }

                if (_firstSelectable.ColorType == ColorType.White)
                {
                    CheckForWhiteCube();
                    return;
                }
                
                if (selectable.IsSelected)
                {
                    return;
                }

                _lastSelectable = selectable;
                _lastSelectablePosition = _lastSelectable.ColorTypeTransform.position;

                SelectedCubes.Add(selectable);
                selectable.SelectDeselect();
                selectable.SelectedCubeLineRenderer.enabled = true;
            }
        }
    }

    private void DeselectCubes()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        MergeSelectableCubes();
        if (!IsMoving)
        {
            foreach (var cube in SelectedCubes)
            {
                cube.SelectDeselect();
                cube.SelectedCubeLineRenderer.enabled = false;
            }
            SelectedCubes.RemoveRange(0, SelectedCubes.Count);
            _firstSelectable = null;
            _lastSelectable = null;
            _emptySelectablesNearFirstSelectable.RemoveRange(0, _emptySelectablesNearFirstSelectable.Count);
        }
    }

   

    private void CheckForWhiteCube()
    {
        OnMakeMove?.Invoke();
        foreach (var cube in AllCubes)
        {
            cube.EmptyCubeLineRenderer.enabled = false;
            if (cube.MeshRenderer.enabled)
            {
                EmptySelectables.Add(cube);
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
        _lastSelectable = null;
        _emptySelectablesNearFirstSelectable.RemoveRange(0, _emptySelectablesNearFirstSelectable.Count);
    }
    
    

    private void SetEmptyCubes()
    {
        foreach (var cube in AllCubes)
        {
            var heading = cube.ColorTypeTransform.position - _firstSelectablePosition;
            var distance = heading.magnitude;
            var direction = heading / distance;
            if (direction.x == 1 || direction.y == 1 || direction.z == 1
                || direction.x == -1 || direction.y == -1 || direction.z == -1)
            {
                _emptySelectablesNearFirstSelectable.Add(cube);
            }
        }
    }

    private void SetLayerMask()
    {
        if (_firstSelectable == null)
        {
            foreach (var emptySelectable in EmptySelectables.Where(emptySelectable => emptySelectable.ColorTypeTransform != null))
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
        
        EmptySelectables.AddRange(cubes);
        // _throwingObject.Throw(_firstSelectable, _mergedCubes, cubes);
    }
    }
}