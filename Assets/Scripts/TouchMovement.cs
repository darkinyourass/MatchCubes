using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
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

    private int _selectableLayerMask;
    private int _ignoreLayerMask;
        
    private Vector3 _currentPosition;
    private Vector3 _secondPosition;

    [SerializeField] private List<ColorView> _emptySelectablesNearCurrentSelectable = new ();

    [SerializeField] private List<ISelectable> _emptySelectables = new();

    public event Action<ISelectable, ISelectable> OnMatchCubes;
    public event Action OnMatchCubesToProgressBar;

    [SerializeField] public List<ColorView> _colorViews = new ();

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
            // colorView.OnMouseDownEvent += ClickDown;
            // colorView.OnMouseOverEvent += ClickOver;
            // colorView.OnMouseUpEvent += ClickUp;
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
                _currentSelectable.SetCurrentSelectableMaterial();
                _currentPosition = _currentSelectable.ColorTypeTransform.position;
                SetEmptyCubes();
                _selection = null;
                break;
            case {IsSelected: true}:
            {
                _currentSelectable.Select();
                _secondSelectable = _selection;
                _currentSelectable.SetMaterial(_currentSelectable.ColorType);
                _secondSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                _secondPosition = _secondSelectable.ColorTypeTransform.position;
                var currentType = _currentSelectable.ColorType;
                var secondType = _secondSelectable.ColorType;
                CheckSelectables(currentType, secondType);
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
            _currentSelectable.ColorTypeTransform.position = Vector3.Lerp(startPosition, secondPosition, timeElapsed / _movementDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _currentSelectable.ColorTypeTransform.position = secondPosition;
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
                emptySelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
            }
        }
        else if (_currentSelectable == null)
        {
            foreach (var emptySelectable in _emptySelectables)
            {
                emptySelectable.ColorTypeTransform.gameObject.layer = _ignoreLayerMask;
            }
        }
    }

    private void CheckSelectables(ColorType currentType, ColorType secondType)
    {
        if (_currentSelectable != _secondSelectable)
        {
            if (Vector3.Distance(_currentPosition, _secondPosition) <= 1f)
            {
                if (_secondSelectable != null)
                {
                    if (currentType == secondType && _currentSelectable.MeshRenderer.enabled && _secondSelectable.MeshRenderer.enabled)
                    {
                        _currentSelectable.MeshRenderer.enabled = false;
                        _secondSelectable.MeshRenderer.enabled = false;
                        _currentSelectable.ColorTypeTransform.position = _currentPosition;
                        _secondSelectable.ColorTypeTransform.position = _secondPosition;
                        AudioManager.Instance.PlayAudioClip(_matchAudioClip);
                        _emptySelectables.Add(_currentSelectable);
                        _emptySelectables.Add(_secondSelectable);
                        OnMatchCubes?.Invoke(_currentSelectable, _secondSelectable);
                        OnMatchCubesToProgressBar?.Invoke();
                        
                        SetSelectablesNull();
                    }
                    else if (_secondSelectable.MeshRenderer.enabled == false)
                    {
                        StartCoroutine(MoveCubeCo(_secondPosition));
                        _secondSelectable.ColorTypeTransform.position = _currentPosition;
                        _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                    }
                    else if (currentType != secondType)
                    {
                        AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
                        _currentSelectable.ColorTypeTransform.position = _currentPosition;
                        _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                        SetSelectablesNull();
                    }
                }
            }
            else
            {
                _currentSelectable.ColorTypeTransform.position = _currentPosition;
                _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
                SetSelectablesNull();
                AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
            }
        }
        else if (_currentSelectable == _secondSelectable)
        {
            _currentSelectable.ColorTypeTransform.position = _currentPosition;
            _currentSelectable.ColorTypeTransform.gameObject.layer = _selectableLayerMask;
            SetSelectablesNull();
            AudioManager.Instance.PlayAudioClip(_wrongAudioClip);
        }
    }
}
