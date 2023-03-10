using System.Collections;
using Cubes.ObjectPooling.BombPool;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Common.View
{
    public class BombSpawner : MonoBehaviour
    {
        
        [Header("Objects")]
        [SerializeField] private Bomb _bombPrefab;
        [SerializeField]private Transform _target;
        [SerializeField] private Vector3 _spawnPosition;

        private Camera _camera;

        [Header("Move duration and cooldown")]
        [SerializeField] private float _movementDuration;
        [SerializeField] private float _maxCooldown;
        [SerializeField] private float _currentCooldown;

        [Header("UI")]
        [SerializeField] private Image _fill;
        [SerializeField] private float _bonusCost;
        
        private UIStateMachine _uiStateMachine;
        private CoinsHolder _coinsHolder;
        private BombPool _bombPool;

        private Rigidbody _bombRigidBody;

        private bool IsCoolDown { get; set; }

        [Inject]
        private void Constructor(UIStateMachine uiStateMachine, CoinsHolder coinsHolder)
        {
            _uiStateMachine = uiStateMachine;
            _coinsHolder = coinsHolder;
        }

        private void OnEnable()
        {
            _camera = FindObjectOfType<Camera>();
            _bombPool = FindObjectOfType<BombPool>();
            _uiStateMachine.OnBombButtonClick += Throw;
        }

        private void Throw()
        {
            if (!IsCoolDown)
            {
                _coinsHolder.UpdateValue((int)_bonusCost);
                StartCoroutine(ThrowBombCo());
                StartCoroutine(StartCooldownCo());
            }
        }

        private void SpawnDamageBall(Vector3 spawnPosition)
        {
            var objectPool = _bombPool.Get();
            
            if (objectPool == null) return;
            _bombRigidBody = objectPool.GetComponent<Rigidbody>();
            objectPool.transform.position = spawnPosition;
            // objectPool.transform = spawnPosition;
            objectPool.gameObject.SetActive(true);
        }

        private IEnumerator ThrowBombCo()
        {
            _spawnPosition = Vector3.zero;
            float timeElapsed = 0;
            _spawnPosition = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 6f));

            // var position = _spawnPosition.position;
            SpawnDamageBall(_spawnPosition);
            _bombRigidBody.velocity = Vector3.zero;
            
            var targetPosition = _target.position;
            var direction = (targetPosition - _spawnPosition).normalized;
            var distance = Vector3.Distance(_spawnPosition, targetPosition);
            var force = direction * distance / _movementDuration;
            
            
            _bombRigidBody.AddForce(force, ForceMode.Acceleration);
            // timeElapsed += Time.deltaTime;
            while (timeElapsed < _movementDuration)
            {
                _bombRigidBody.AddForce(force, ForceMode.Acceleration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // StartCoroutine(TimerCo());
        }

        private IEnumerator TimerCo()
        {
            yield return new WaitForSeconds(0.5f);
            _bombRigidBody.velocity = Vector3.zero;
        }
        
        private IEnumerator StartCooldownCo()
        {
            IsCoolDown = true;
            _currentCooldown = _maxCooldown;
            while (_currentCooldown > 0f)
            {
                _fill.fillAmount = _currentCooldown / _maxCooldown;
                _currentCooldown -= Time.deltaTime;
                yield return null;
            }
           
            IsCoolDown = false;
            _fill.fillAmount = 0f;
        }
        
        
    }
}