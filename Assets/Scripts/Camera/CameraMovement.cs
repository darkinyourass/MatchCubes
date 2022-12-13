using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [SerializeField] private float _distanceToTarget;
    private Camera _camera;

    private Vector3 _position;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
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
            _camera.transform.Rotate(new Vector3(0, 1, 0),  rotationY, Space.World);
            
            _camera.transform.Translate(new Vector3(0, 0, -_distanceToTarget));

            _position = newPosition;
        }
    }
}
