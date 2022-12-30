using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.View;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TouchMovement : MonoBehaviour
{
    [Inject]
    private TestGrid _testGrid;
    
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

    private ColorView _cubeHit;

    [SerializeField] private List<ColorView> _emptySelectablesNearCurrentSelectable = new ();

    private List<ISelectable> _emptySelectables = new();

    private readonly List<ISelectable> _raycastSelectables = new();
    
    private bool IsMoving { get; set; }

    public event Action<List<ISelectable>> OnMatchingCubes;
    
    // public event Action OnMatchCubesToProgressBar;
    // private ColorView _colorView;

    [SerializeField] public List<ColorView> _colorViews = new ();
    
    private RaycastHit[] _hits;
    
    public List<ISelectable> EmptySelectables { get => _emptySelectables; set => _emptySelectables = value; }

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
                _currentSelectable.Animator.SetBool(_currentSelectable.SelectingAnimationHash , true);
                _currentPosition = _currentSelectable.ColorTypeTransform.position;
                SetEmptyCubes();
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
        foreach (var colorView in _colorViews.Where
                 (colorView => Vector3.Distance(_currentPosition, colorView.ColorTypeTransform.position) <= 1f))
        {
            _emptySelectablesNearCurrentSelectable.Add(colorView);
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
            foreach (var emptySelectable in _emptySelectables)
            {
                if (emptySelectable.ColorTypeTransform != null)
                {
                    emptySelectable.ColorTypeTransform.gameObject.layer = _ignoreLayerMask;
                }
            }
        }
    }

    private void CheckSelectablesBetweenCurrentAndSecond()
    {
        var heading = _secondPosition - _currentPosition;
        var distance = heading.magnitude;
        var direction = heading / distance;
        // if (direction.x % 1 != 0 || direction.y % 1 != 0 || direction.z % 1 != 0)
        // {
        //     AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
        //     SetSelectablesNull();
        //     return;
        // }
        var allCubes = Physics.RaycastAll(_currentPosition, direction, Vector3.Distance(_currentPosition, _secondPosition),
            _layerMask);
        
        _raycastSelectables.Add(_currentSelectable);
        foreach (var cube in allCubes)
        {
            _cubeHit = cube.collider.GetComponent<ColorView>();
            _raycastSelectables.Add(_cubeHit);
        }

        foreach (var selectable in _raycastSelectables)
        {
            if ((selectable.ColorType != _currentSelectable.ColorType && selectable.MeshRenderer.enabled) ||
                _raycastSelectables.Count % 2 != 0)
            {
                AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
            }

            if (selectable.ColorType == _currentSelectable.ColorType && selectable.MeshRenderer.enabled &&
                _raycastSelectables.Count % 2 == 0) continue;
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
        OnMatchingCubes?.Invoke(_raycastSelectables);
        AudioManager.Instance.PlayAudioClip(_matchAudioClip);
        _currentSelectable.MeshRenderer.enabled = false;
        _currentSelectable.ColorTypeTransform.position = _currentPosition;  
        _emptySelectables.Add(_currentSelectable);
        _emptySelectables.AddRange(_raycastSelectables);
        SetSelectablesNull();
        _raycastSelectables.RemoveRange(0, _raycastSelectables.Count);
    }

    private void MoveCubeToEmptyPosition()
    {
        if (!(Vector3.Distance(_currentPosition, _secondPosition) <= 1f)) return;
        if (_secondSelectable.MeshRenderer.enabled) return;
        StartCoroutine(MoveCubeCo(_secondPosition));
        _secondSelectable.ColorTypeTransform.position = _currentPosition;
        _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
    }

    
    // private void CheckSelectables(ColorType currentType, ColorType secondType)
    // {
    //     if (_currentSelectable != _secondSelectable)
    //     {
    //         if (Vector3.Distance(_currentPosition, _secondPosition) <= 1f)
    //         {
    //             if (_secondSelectable != null)
    //             {
    //                 if (currentType == secondType && _currentSelectable.MeshRenderer.enabled && _secondSelectable.MeshRenderer.enabled)
    //                 {
    //                     _currentSelectable.MeshRenderer.enabled = false;
    //                     _secondSelectable.MeshRenderer.enabled = false;
    //                     _currentSelectable.ColorTypeTransform.position = _currentPosition;
    //                     _secondSelectable.ColorTypeTransform.position = _secondPosition;
    //                     AudioManager.Instance.PlayAudioClip(_matchAudioClip);
    //                     _emptySelectables.Add((ColorView)_currentSelectable);
    //                     _emptySelectables.Add((ColorView)_secondSelectable);
    //                     OnMatchCubes?.Invoke(_currentSelectable, _secondSelectable);
    //                     OnMatchCubesToProgressBar?.Invoke();
    //                     
    //                     SetSelectablesNull();
    //                 }
    //                 else if (_secondSelectable.MeshRenderer.enabled == false)
    //                 {
    //                     StartCoroutine(MoveCubeCo(_secondPosition));
    //                     _secondSelectable.ColorTypeTransform.position = _currentPosition;
    //                     _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
    //                 }
    //                 else if (currentType != secondType)
    //                 {
    //                     AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
    //                     _currentSelectable.ColorTypeTransform.position = _currentPosition;
    //                     _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
    //                     SetSelectablesNull();
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             _currentSelectable.ColorTypeTransform.position = _currentPosition;
    //             _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
    //             SetSelectablesNull();
    //             AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
    //         }
    //     }
    //     else if (_currentSelectable == _secondSelectable)
    //     {
    //         _currentSelectable.ColorTypeTransform.position = _currentPosition;
    //         _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
    //         SetSelectablesNull();
    //         AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
    //     }
    // }
}
