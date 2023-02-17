using System;
using System.Collections;
using System.Collections.Generic;
using Cubes;
using Cubes.ObjectPooling.BombPool;
using UnityEngine;
using Zenject;

namespace Common.View
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Collider _collision;

        private float _timer;
        [SerializeField] private float _destroyTime = 5f;

        private TouchMovement _touchMovement;

        private WinCondition _winCondition;

        [Inject]
        private void Constructor(TouchMovement touchMovement, WinCondition winCondition)
        {
            _touchMovement = touchMovement;
            _winCondition = winCondition;
        }

        private BombPool _bombPool;
        
        private HashSet<ISelectable> _triggeredCubes = new HashSet<ISelectable>();

        private bool IsCollided { get; set; }
        
        private bool IsTriggered { get; set; }


        private void Start()
        {
            
        }

        private void OnEnable()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ignore Raycast"), LayerMask.NameToLayer("Bomb"), true);
            _winCondition = FindObjectOfType<WinCondition>();
            _touchMovement = FindObjectOfType<TouchMovement>();
            _bombPool = FindObjectOfType<BombPool>();
            _timer = _destroyTime;
        }

        private void Update()
        {
            while (_timer > 0)
            {
                _timer -= Time.deltaTime;
                return;
            }
            _bombPool.ReturnToPool(this);
        }

        private void OnTriggerEnter(Collider other)
        {   
            var cube = other.GetComponent<ISelectable>();
            if (cube != null && !_triggeredCubes.Contains(cube) && cube.MeshRenderer.enabled && cube.ColorType != ColorType.White)
            {
                _triggeredCubes.Add(cube);
                _winCondition.EmptySelectables.Add(cube);
                _touchMovement.EmptyCubes.Add(cube);
                cube.ParticleSystem.Play();
                cube.MeshRenderer.enabled = false;
                cube.EmptyCubeLineRenderer.enabled = true;
                IsTriggered = true;
                Debug.Log("Triggered");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var cube = collision.collider.GetComponent<ISelectable>();

            if (cube != null )
            {
                switch (cube.MeshRenderer.enabled)
                {
                    case false:
                        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ignore Raycast"),
                            LayerMask.NameToLayer("Bomb"), true);
                        break;
                    case true:
                    {
                        if (!IsCollided)
                        {
                            IsCollided = true;
                            _collider.enabled = true;
                            StartCoroutine(DestroyBombCo());
                        }
                        break;
                    }
                }
            }
        }

        private IEnumerator DestroyBombCo()
        {
            yield return new WaitForSeconds(0.15f);
            _collider.enabled = false;
            IsCollided = false; 
            _bombPool.ReturnToPool(this);
        }
    }
}