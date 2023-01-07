using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _distanceToTarget;
    private Camera _camera;
    private Touch _touch;
    
    private float _currentX;
    private float _currentY;

    private TestGrid _testGrid;

    private Vector3 _currentRotation;

    [Inject]
    private void Constructor(TestGrid testGrid)
    {
        _testGrid = testGrid;
    }

    private Vector3 _position;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _target.position = new Vector3(_testGrid.Size / 2f - 0.5f, _testGrid.Size / 2f - 0.5f,
            _testGrid.Size / 2f - 0.5f);
        _camera.transform.position = _target.position;
        _camera.transform.Translate(new Vector3(0, 0, -_distanceToTarget));
    }

    private void Update()
    {
        // if (EventSystem.current.IsPointerOverGameObject())
        // {
        //     return;
        // }

        Debug.Log(_camera.gameObject.transform.rotation.z);
        // var sensitivityX = 6f;
        // var sensitivityY = 6f;
        //
        // if (Input.GetMouseButton(1))
        // {
        //     _currentX += Input.GetAxis("Mouse X") * sensitivityX;
        //     _currentY += Input.GetAxis("Mouse Y") * sensitivityY;
        // }
        // MoveCamera();
        // MoveCameraWithMouse();
        // RotateCamera();
    }

    private void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            // Get the cursor position in screen space
            Vector3 cursorPos = Input.mousePosition;

            // Convert the cursor position to world space
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(cursorPos);

            // Calculate the direction vector from the object to the cursor
            Vector3 direction = worldPos - _target.position;

            // Calculate the rotation from the object's forward direction to the direction vector
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Calculate the camera's position based on the desired distance and rotation
            Vector3 position = _target.position + rotation * Vector3.forward * _distanceToTarget;

            // Apply the rotation and position to the camera's transform
            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private void MoveCamera()
    {
        if (Input.touchCount != 1) return;
        _touch = Input.GetTouch(0);

        switch (_touch.phase)
        {
            case TouchPhase.Began:
                _position = _camera.ScreenToViewportPoint(_touch.position);
                break;
            case TouchPhase.Moved:
            {
                var newPosition = _camera.ScreenToViewportPoint(_touch.position);
                var direction = _position - newPosition;

                var rotationX = direction.y * 180;
                var rotationY = -direction.x * 180;

                _camera.transform.position = _target.position;
                
                _camera.transform.Rotate(new Vector3(1, 0, 0), rotationX);
                _camera.transform.Rotate(new Vector3(0, 1, 0), rotationY, Space.World);

                _camera.transform.Translate(new Vector3(0, 0, -_distanceToTarget));

                _position = newPosition;
                break;
            }
            case TouchPhase.Ended:
                break;
        }
    }

    private void MoveCameraWithMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _position = _camera.ScreenToViewportPoint(Input.mousePosition);
        }

        else if (Input.GetMouseButton(1))
        {
            var newPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
            var direction = _position - newPosition;

            var rotationX = direction.y * 180;
            var rotationY = -direction.x * 180;

            _camera.transform.position = _target.position;

            _camera.transform.Rotate(new Vector3(1, 0, 0), rotationX);
            _camera.transform.Rotate(new Vector3(0, 1, 0), rotationY, Space.World);
            //
            // if (_camera.gameObject.transform.rotation.z <= -180f)
            // {
            //     _camera.transform.Rotate(new Vector3(1, 0, 0), rotationX);
            //     _camera.transform.Rotate(new Vector3(0, 1, 0), -rotationY, Space.World);
            // }
            // else
            // {
            //     _camera.transform.Rotate(new Vector3(1, 0, 0), rotationX);
            //     _camera.transform.Rotate(new Vector3(0, 1, 0), rotationY, Space.World);
            // }
            
            _camera.transform.Translate(new Vector3(0, 0, -_distanceToTarget));

            _position = newPosition;
        }
    }

    private void LateUpdate()
    {
        // transform.position = _target.position + Quaternion.Euler(_currentY, _currentX, 0) * new Vector3(0, 0, -_distanceToTarget);
        // transform.LookAt(_target.position);
        // transform.LookAt();
        
    }
}
