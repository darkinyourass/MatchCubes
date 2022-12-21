using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [SerializeField] private float _distanceToTarget;
    private Camera _camera;
    private Touch _touch;

    private Vector3 _position;
    
    public static bool IsCameraLocked { get; set; }
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.transform.position = _target.position;
        _camera.transform.Translate(new Vector3(0, 0, -_distanceToTarget));
    }

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (!IsCameraLocked)
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
    }
}
