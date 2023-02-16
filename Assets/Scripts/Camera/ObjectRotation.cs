using System;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Camera
{
    public class ObjectRotation : MonoBehaviour
    {
        public bool IsRotating { get; set; }
        public bool IsZooming { get; set; }
        private Touch _touch;
        private Touch _firstTouch;

        private TouchMovement _touchMovement;

        private TestGrid _testGrid;
        
        [Inject]
        private void Constructor(TouchMovement touchMovement, TestGrid testGrid)
        {
            _touchMovement = touchMovement;
            _testGrid = testGrid;
        }
        
        // private Vector3 _lastMousePosition;
        //
        // private void LateUpdate()
        // {
        //     if (!_touchMovement.IsSelectingCubes)
        //     {
        //         MoveWithMouse();
        //         MoveWithTouch();
        //     }
        // }
        //
        // private void MoveWithMouse()
        // {
        //     if (Input.GetMouseButtonDown(1))
        //     {
        //         _isRotating = true;
        //         _lastMousePosition = Input.mousePosition;
        //     }
        //     if (Input.GetMouseButtonUp(1))
        //     {
        //         _isRotating = false;
        //     }
        //
        //     if (!_isRotating) return;
        //     var currentMousePosition = Input.mousePosition;
        //     var delta = currentMousePosition - _lastMousePosition;
        //     _lastMousePosition = currentMousePosition;
        //
        //     delta *= _rotationSpeed;
        //
        //     transform.Rotate(Vector3.up, delta.x);
        //     transform.Rotate(Vector3.right, -delta.y, Space.World);
        // }
        //
        // private void MoveWithTouch()
        // {
        //     if (Input.touchCount != 1) return;
        //     _touch = Input.GetTouch(0);
        //     
        //     switch (_touch.phase)
        //     {
        //         case TouchPhase.Began:
        //             _isRotating = true;
        //             _lastMousePosition = Input.mousePosition;
        //             break;
        //         case TouchPhase.Moved:
        //             if (_isRotating)
        //             {
        //                 var currentMousePosition = Input.mousePosition;
        //                 var delta = currentMousePosition - _lastMousePosition;
        //                 _lastMousePosition = currentMousePosition;
        //                 delta *= _rotationSpeed;
        //                 transform.Rotate(Vector3.up, delta.x);
        //                 transform.Rotate(Vector3.right, -delta.y, Space.World);
        //             }
        //             break;
        //         case TouchPhase.Ended:
        //             _isRotating = false;
        //             break;
        //     }
        // }
        
        [Header("Rotation values")]
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance;
        [SerializeField] private float _xRotationSpeed;
        [SerializeField] private float _yRotationSpeed;
        [SerializeField] private float _yMinLimit;
        [SerializeField] private float _yMaxLimit;
        private float _xRotation;
        private float _yRotation;


        [Header("Zooming values")]
        [SerializeField] private float _minZoomDistance;
        [SerializeField] private float _maxZoomDistance;
        [SerializeField] private float _zoomSpeed;
        
        private void Start()
        {
            var angles = transform.eulerAngles;
            _xRotation = angles.y;
            _yRotation = angles.x;
        }

        

        private void LateUpdate()
        {
            // Debug.Log($"Zoom: {IsZooming}");
            // Debug.Log($"Rotate: {IsRotating}");
            // Debug.Log($"Selection: {_touchMovement.IsSelectingCubes}");
            
            // Debug.Log(_touch);
            if (!_touchMovement.IsSelectingCubes && _testGrid.IsCameraRotatingAvailable)
            {
                MoveWithTouch();
                MoveWithMouse();
            }

        }
        
        private void MoveWithMouse()
        {
            if (_target)
            {
                if (Input.GetMouseButton(1))
                {
                    IsRotating = true;
                    _xRotation += Input.GetAxis("Mouse X") * _xRotationSpeed * 0.02f;
                    _yRotation -= Input.GetAxis("Mouse Y") * _yRotationSpeed * 0.02f;
                    _yRotation = ClampAngle(_yRotation, _yMinLimit, _yMaxLimit);
                }
        
                if (Input.GetMouseButtonUp(1))
                {
                    IsRotating = false;
                }
        
                Quaternion rotation = Quaternion.Euler(_yRotation, _xRotation, 0);
                Vector3 position = rotation * new Vector3(0.0f, 0.0f, -_distance) + _target.position;
        
                transform.rotation = rotation;
                transform.position = position;
            }
        }

        private void MoveWithTouch()
        {
            if (Input.touchCount == 1)
            {
                _touch = Input.GetTouch(0);
                _firstTouch = _touch;

                switch (_touch.phase)
                {
                    case TouchPhase.Began:
                        IsRotating = true;
                        break;
                    case TouchPhase.Moved:
                        if (IsRotating)
                        {
                            _xRotation += Input.GetAxis("Mouse X") * _xRotationSpeed * 0.02f;
                            _yRotation -= Input.GetAxis("Mouse Y") * _yRotationSpeed * 0.02f;
                            _yRotation = ClampAngle(_yRotation, _yMinLimit, _yMaxLimit);
                            
                            Quaternion rotation = Quaternion.Euler(_yRotation, _xRotation, 0);
                            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -_distance) + _target.position;

                            transform.rotation = rotation;
                            transform.position = position;
                        }
                        break;
                    case TouchPhase.Ended:
                        IsRotating = false;
                        break;
                }
            }
            
            else if (Input.touchCount == 2)
            {
                IsRotating = false;
                ZoomCamera();
            }
        }


        private void ZoomCamera()
        {
            // if (Input.touchCount != 2) return;
            IsZooming = true;
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            _distance += deltaMagnitudeDiff * _zoomSpeed * 0.01f;
            _distance = Mathf.Clamp(_distance, _minZoomDistance, _maxZoomDistance);
            
            Quaternion rotation = Quaternion.Euler(_yRotation, _xRotation, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -_distance) + _target.position;
        
            transform.rotation = rotation;
            transform.position = position;
            
            if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended)
            {
                IsZooming = false;
                // IsRotating = true;
            }
        }
        
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
            {
                angle += 360f;
            }
            if (angle > 360f)
            {
                angle -= 360f;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}