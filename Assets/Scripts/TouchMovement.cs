using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.View;
using DefaultNamespace;
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

    private ISelectable _currentSelectable;
    private ISelectable _secondSelectable;
    private ISelectable _selection;

    private Touch _touch;

    [SerializeField] private LayerMask _layerMask;
    
    private int _selectableLayerMask;
    private int _ignoreLayerMask;
        
    private Vector3 _currentPosition;
    private Vector3 _secondPosition;
    
    public event Action<List<ISelectable>> OnMatchingCubes;
    public event Action OnMakeMove;

    public event Action OnTutorialCubeClick;
    public event Action OnTutorialSecondCubeClick;
    public event Action OnTutorialThirdClick;
    public event Action OnTutorialFourthClick;
    
    private RaycastHit[] _hits;

    private ColorView _cubeHit;

    private readonly List<ISelectable> _emptySelectablesNearCurrentSelectable = new ();

    private List<ISelectable> _emptySelectables = new();

    private readonly List<ISelectable> _raycastSelectables = new();
    
    public List<ISelectable> _colorViews = new ();

    private readonly List<ISelectable> _anotherList = new ();
    
    private readonly List<ISelectable> _mergedCubes = new ();
    
    private bool IsMoving { get; set; }
    
    public List<ISelectable> EmptySelectables { get => _emptySelectables; set => _emptySelectables = value; }
    
    [Inject]
    private void Constructor(TestGrid testGrid, UIStateMachine stateMachine, CoinsHolder coinsHolder)
    {
        _testGrid = testGrid;
        _uiStateMachine = stateMachine;
        _coinsHolder = coinsHolder;
    }

    private void Start()
    {
        _selectableLayerMask = LayerMask.NameToLayer("Selectables");
        _ignoreLayerMask = LayerMask.NameToLayer("Ignore Raycast");
        _testGrid.OnGameStarted += StartGame;
    }

    private void Update()
    {
        SetLayerMask();
    }

    private void StartGame()
    {
        foreach (var colorView in _colorViews)
        {
            colorView.OnMouseDownAsButton += ClickOnCube;
        }
    }

    private void CheckForCore()
    {
        OnMakeMove?.Invoke();
        foreach (var cube in _colorViews)
        {
            cube.LineRenderer.enabled = false;
            if (cube.MeshRenderer.enabled)
            {
                _emptySelectables.Add(cube);
                _anotherList.Add(cube);
                cube.MeshRenderer.enabled = false;
                cube.ParticleSystem.Play();
            }
        }
        OnMatchingCubes?.Invoke(_anotherList);
        _anotherList.RemoveRange(0, _anotherList.Count);
        SetSelectablesNull();
    }
    
    private void ClickOnCube(ISelectable selectable)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        _selection = selectable;
        switch (_currentSelectable)
        {
            case null when _selection.MeshRenderer.enabled:
                _selection.Select();
                _currentSelectable = _selection;
                if (_currentSelectable.ColorType == ColorType.White)
                {
                    CheckForCore();
                    return;
                }

                _currentSelectable.Animator.SetBool(_currentSelectable.SelectingAnimationHash , true);
                _currentPosition = _currentSelectable.ColorTypeTransform.position;
                SetEmptyCubes();
                switch (_testGrid.isTutorialFinished)
                {
                    case false when !TestGrid.IsFirstClicked && _currentSelectable.ColorTypeTransform.position == new Vector3(0, 1, 0):
                        OnTutorialCubeClick?.Invoke();
                        break;
                    case false when !TestGrid.IsThirdClicked && _currentSelectable.ColorTypeTransform.position == new Vector3(1, 0, 0):
                        OnTutorialThirdClick?.Invoke();
                        break;
                }
                _selection = null;
                break;
            case {IsSelected: true}:
            {
                _currentSelectable.Animator.SetBool(_currentSelectable.SelectingAnimationHash , false);
                _currentSelectable.Select();
                _secondSelectable = _selection;
                _currentSelectable.SetMaterial(_currentSelectable.ColorType);
                _secondSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                _secondPosition = _secondSelectable.ColorTypeTransform.position;
                switch (_testGrid.isTutorialFinished)
                {
                    case false when !TestGrid.IsSecondClicked && _secondSelectable.ColorTypeTransform.position == new Vector3(1, 1, 0):
                        OnTutorialSecondCubeClick?.Invoke();
                        break;
                    case false when !TestGrid.IsFourthClicked && _secondSelectable.ColorTypeTransform.position == new Vector3(1, 1, 0):
                        OnTutorialFourthClick?.Invoke();
                        break;
                }
                MoveCubeToEmptyPosition();
                CheckSelectablesBetweenCurrentAndSecond();
                _emptySelectablesNearCurrentSelectable.RemoveRange(0, _emptySelectablesNearCurrentSelectable.Count);
                
                break;
            }
        }
    }

    private IEnumerator MoveCubeCo(Vector3 secondPosition)
    {
        float timeElapsed = 0;
        var startPosition = _currentSelectable.ColorTypeTransform.position;
        while (timeElapsed < _movementDuration)
        {
            IsMoving = true;
            _currentSelectable.ColorTypeTransform.position = Vector3.Lerp(startPosition, secondPosition, timeElapsed / _movementDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        OnMakeMove?.Invoke();
        _currentSelectable.ColorTypeTransform.position = secondPosition;
        IsMoving = false;
        SetSelectablesNull();
    }

    private void SetSelectablesNull()
    {
        _secondSelectable = null;
        _currentSelectable = null;
        _selection = null;
    }

    private void SetEmptyCubes()
    {
        foreach (var colorView in _colorViews)
        {
            var heading = colorView.ColorTypeTransform.position - _currentPosition;
            var distance = heading.magnitude;
            var direction = heading / distance;
            if (direction.x == 1 || direction.y == 1 || direction.z == 1
                || direction.x == -1 || direction.y == -1 || direction.z == -1)
            {
                _emptySelectablesNearCurrentSelectable.Add(colorView);
            }
        }
    }

    private void SetLayerMask()
    {
        if (_currentSelectable != null)
        {
            foreach (var emptySelectable in _emptySelectablesNearCurrentSelectable)
            {
                if (emptySelectable.ColorTypeTransform != null)
                {
                    emptySelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                }
            }
        }
        else if (_currentSelectable == null)
        {
            foreach (var emptySelectable in _emptySelectables.Where(emptySelectable => emptySelectable.ColorTypeTransform != null))
            {
                emptySelectable.ColorTypeTransform.gameObject.layer = _ignoreLayerMask;
            }
        }
    }

    private void CheckForMatchingSelectablesCount()
    {
        switch (_raycastSelectables.Count)
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
        _colorViews = _colorViews.OrderBy(x => Random.value).ToList();
        var distinctColors = _colorViews.Select(x => x.ColorType).ToList();

        for (var i = 0; i < pairs; i++)
        {
            var color = distinctColors[Random.Range(0, distinctColors.Count)];
            
            var cubesWithColor = _colorViews.Where(x =>
                x.ColorType == color && x.MeshRenderer.enabled && x.ColorType != ColorType.White &&
                !_raycastSelectables.Contains(x)).ToList();

            if (color == ColorType.White)
            {
                i--;
                continue;
            }

            if (cubesWithColor.Count < 2) continue;
            MergeCubes(cubesWithColor[0], cubesWithColor[1]);
        }
    }

    private void MergeCubes(ISelectable cube1, ISelectable cube2)
    {                 
        _mergedCubes.Add(cube1);
        _mergedCubes.Add(cube2);
        
        OnMatchingCubes?.Invoke(_mergedCubes);
        
        cube1.MeshRenderer.enabled = false;
        cube2.MeshRenderer.enabled = false;
        
        cube1.ParticleSystem.Play();
        cube2.ParticleSystem.Play();
        
        _emptySelectables.AddRange(_mergedCubes);
        
        cube1.ParticleSystem.Play();
        cube2.ParticleSystem.Play();
        
        cube1.LineRenderer.enabled = true;
        cube2.LineRenderer.enabled = true;
        
        Debug.Log(_emptySelectables.Count);
        _mergedCubes.RemoveRange(0, _mergedCubes.Count);
    }

    private void CheckSelectablesBetweenCurrentAndSecond()
    {
        var heading = _secondPosition - _currentPosition;
        var distance = heading.magnitude;
        var direction = heading / distance;

        var allCubes = Physics.RaycastAll(_currentPosition, direction,
            Vector3.Distance(_currentPosition, _secondPosition),
            _layerMask);
        
        if (direction.x != 1 && direction.y != 1 && direction.z != 1 && direction.x != -1 && direction.y != -1 && direction.z != -1)
        {
            AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
            SetSelectablesNull();
            return;
        }

        foreach (var cube in allCubes)
        {
            _cubeHit = cube.collider.GetComponent<ColorView>();
            _raycastSelectables.Add(_cubeHit);
        }
        _raycastSelectables.Add(_currentSelectable);

        foreach (var selectable in _raycastSelectables)
        {
            if (selectable.ColorType != _currentSelectable.ColorType && selectable.MeshRenderer.enabled)
            {
                AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
            }

            if (selectable.ColorType == _currentSelectable.ColorType && selectable.MeshRenderer.enabled) continue;
            
            _raycastSelectables.RemoveRange(0, _raycastSelectables.Count);  
            
            if (!IsMoving)
            {
                SetSelectablesNull();
            }
            return;
        }

        foreach (var selectable in _raycastSelectables)
        {
            selectable.MeshRenderer.enabled = false;
        }
        EmptySelectables.AddRange(_raycastSelectables);
        OnMatchingCubes?.Invoke(_raycastSelectables);
        AudioManager.Instance.PlayAudioClip(_matchAudioClip);
        OnMakeMove?.Invoke();
        _currentSelectable.MeshRenderer.enabled = false;
        _currentSelectable.ColorTypeTransform.position = _currentPosition;  
        
        foreach (var selectable in _raycastSelectables)
        {
            selectable.ParticleSystem.Play();
            selectable.LineRenderer.enabled = true;
        }
        
        _currentSelectable.ParticleSystem.Play();
        _currentSelectable.LineRenderer.enabled = true;
        CheckForMatchingSelectablesCount();
        SetSelectablesNull();
        _raycastSelectables.RemoveRange(0, _raycastSelectables.Count);
    }

    private void MoveCubeToEmptyPosition()
    {
        var heading = _secondPosition - _currentPosition;
        var distance = heading.magnitude;
        var direction = heading / distance;
        if (_secondSelectable.MeshRenderer.enabled) return;
        if (direction.x != 1 && direction.y != 1 && direction.z != 1 && direction.x != -1 && direction.y != -1 && direction.z != -1)
        {
            AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
            SetSelectablesNull();
            return;
        }
        StartCoroutine(MoveCubeCo(_secondPosition));
        _secondSelectable.ColorTypeTransform.position = _currentPosition;
        _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
    }
}
