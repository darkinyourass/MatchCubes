using System;
using Common;
using Common.View;
using UnityEngine;

namespace DefaultNamespace
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;

        private RaycastHit _hit;
        private ISelectable _currentSelectable;
        private ISelectable _secondSelectable;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out _hit, _layerMask)) return;
                var selection =  _hit.collider.GetComponent<ISelectable>();
                
                if (selection != null && _currentSelectable == null)
                {
                    selection.Select();
                    _currentSelectable = selection;
                    
                }
                else if (selection != null && _currentSelectable != null && _currentSelectable.IsSelected)
                {
                    _currentSelectable.Select();
                    _secondSelectable = selection;
                    _secondSelectable.IsSelected = false;
                    var currentType = _currentSelectable.ColorType;
                    var secondType = _secondSelectable.ColorType;
                    
                    if (Vector3.Distance(_currentSelectable.ColorTypeTransform.position,
                            _secondSelectable.ColorTypeTransform.position) <= 1f)
                    {
                        if (_currentSelectable != _secondSelectable)
                        {
                            if (currentType == secondType)
                            {
                                _currentSelectable.ColorType = ColorType.White;
                                _secondSelectable.ColorType = ColorType.White;
                            }
                            else if (secondType == ColorType.White)
                            {
                                _secondSelectable.ColorType = currentType;
                                _currentSelectable.ColorType = ColorType.White;
                            }
                        
                            else if (currentType != secondType)
                            {
                            
                                _currentSelectable.ColorType = secondType;
                                _secondSelectable.ColorType = currentType;
                            }
                        }
                    }
                    
                    _currentSelectable = null;
                    _secondSelectable = null;
                }
            }
        }
    }
}