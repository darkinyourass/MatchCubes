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
    
    public event Action<List<ISelectable>> OnMatchingCubes;

    public event Action OnMakeMove;
    
    private RaycastHit[] _hits;

    private ColorView _cubeHit;

    private readonly List<ISelectable> _emptySelectablesNearCurrentSelectable = new ();

    private List<ISelectable> _emptySelectables = new();

    private readonly List<ISelectable> _raycastSelectables = new();
    
    public List<ISelectable> _colorViews = new ();

    private readonly List<ISelectable> _anotherList = new List<ISelectable>();
    
    private List<ISelectable> _mergedCubes = new List<ISelectable>();
    
    private bool IsMoving { get; set; }
    
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
                if (_currentSelectable.ColorType == ColorType.White)
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
                    return;
                }
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
                CoinsHolder.CoinsAmount += 2;
                break;
            case 3:
                CoinsHolder.CoinsAmount += 4;
                break;
            case 4:
                CoinsHolder.CoinsAmount += 8;
                FindAndMergeCubes(5);
                break;
            case 5:
                CoinsHolder.CoinsAmount += 12;
                FindAndMergeCubes(8);
                break;
            case 6:
                CoinsHolder.CoinsAmount += 20;
                FindAndMergeCubes(10);
                break;
        }
    }
    
    private void FindAndMergeCubes(int pairs)
    {
        _colorViews = _colorViews.OrderBy(x => Random.value).ToList();
        var distinctColors = _colorViews.Select(x => x.ColorType).ToList();

        for (int i = 0; i < pairs; i++)
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
            if (cubesWithColor.Count >= 2) 
            {
                Debug.Log(cubesWithColor[0].ColorType);
                Debug.Log(cubesWithColor[1].ColorType);
                MergeCubes(cubesWithColor[0], cubesWithColor[1]);
            }
        }
    }

    private void MergeCubes(ISelectable cube1, ISelectable cube2)
    {
        // cube1.ColorTypeTransform.gameObject.SetActive(false);
        // cube2.ColorTypeTransform.gameObject.SetActive(false);
        
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
        // if (!(Vector3.Distance(_currentPosition, _secondPosition) <= 1f)) return;
        // if (_secondSelectable.MeshRenderer.enabled) return;
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
