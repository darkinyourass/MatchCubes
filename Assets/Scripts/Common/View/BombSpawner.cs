using System.Collections;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Common.View
{
    public class BombSpawner : MonoBehaviour
    {
        [SerializeField]private Transform _target;
        private Camera _camera;

        [SerializeField] private Bomb _bombPrefab;
        [SerializeField] private float _movementDuration;

        private UIStateMachine _uiStateMachine;

        [Inject]
        private void Constructor(UIStateMachine uiStateMachine)
        {
            _uiStateMachine = uiStateMachine;
        }

        private void OnEnable()
        {
            _camera = FindObjectOfType<Camera>();
            _uiStateMachine.OnBombButtonClick += Throw;
        }

        private void Throw()
        {
            StartCoroutine(ThrowBomb());
        }

        private IEnumerator ThrowBomb()
        {
            float timeElapsed = 0;
            var startPosition = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 6f));
            var bomb = Instantiate(_bombPrefab, startPosition, Quaternion.identity, transform);

            var bombRigidbody = bomb.GetComponent<Rigidbody>();

            var targetPosition = _target.position;
            var direction = (targetPosition - startPosition).normalized;
            var distance = Vector3.Distance(startPosition, targetPosition);
            var force = direction * distance / _movementDuration;
            
            while (timeElapsed < _movementDuration)
            {
                // bomb.transform.position = Vector3.Lerp
                //     (startPosition, _target.position, timeElapsed / _movementDuration);
                
                bombRigidbody.AddForce(force, ForceMode.Acceleration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}