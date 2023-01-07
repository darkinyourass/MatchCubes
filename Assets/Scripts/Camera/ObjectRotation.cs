using UnityEngine;

namespace Game.Camera
{
    public class ObjectRotation : MonoBehaviour
    {
        private Vector3 _lastMousePosition;
        private bool _isRotating;
        private Touch _touch;

        private void Update()
        {
             MoveWithMouse();
             MoveWithTouch();
        }

        private void MoveWithMouse()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _isRotating = true;
                _lastMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(1))
            {
                _isRotating = false;
            }

            if (_isRotating)
            {
                var currentMousePosition = Input.mousePosition;
                var delta = currentMousePosition - _lastMousePosition;
                _lastMousePosition = currentMousePosition;

                transform.Rotate(Vector3.up, delta.x);
                transform.Rotate(Vector3.right, -delta.y, Space.World);
            }
        }

        private void MoveWithTouch()
        {
            if (Input.touchCount != 1) return;
            _touch = Input.GetTouch(0);
            
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    _isRotating = true;
                    _lastMousePosition = Input.mousePosition;
                    break;
                case TouchPhase.Moved:
                    if (_isRotating)
                    {
                        var currentMousePosition = Input.mousePosition;
                        var delta = currentMousePosition - _lastMousePosition;
                        _lastMousePosition = currentMousePosition;

                        transform.Rotate(Vector3.up, delta.x);
                        transform.Rotate(Vector3.right, -delta.y, Space.World);
                    }
                    break;
                case TouchPhase.Ended:
                    _isRotating = false;
                    break;
            }
        }
    }
}