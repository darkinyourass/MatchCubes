using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.View
{ public class ThrowingObject: MonoBehaviour
    {
        [field: SerializeField] public float MovementDuration { get; set; }
        [SerializeField] private GameObject _throwingObjectPrefab;
        
        private bool IsMoving { get; set; }
        private readonly List<GameObject> _spheres = new ();
        private GameObject _sphere;

        private float _maxDistance;


        public void Throw(ISelectable firstSelectable, List<ISelectable> objectsToThrow, List<ISelectable> cubes)
        {
            StartCoroutine(ThrowObjectCo(firstSelectable, objectsToThrow, cubes));
        }

        private IEnumerator ThrowObjectCo(ISelectable firstSelectable, List<ISelectable> objectsToThrow, List<ISelectable> cubes)
        {
            float timeElapsed = 0;
            var startPosition = firstSelectable.ColorTypeTransform.position;
         
            for (var i = 0; i < objectsToThrow.Count; i++)
            {
                _sphere = Instantiate(_throwingObjectPrefab, startPosition, Quaternion.identity, transform);
                _spheres.Add(_sphere);
            }
            
            _maxDistance = 0;
            foreach (var distance 
                     in objectsToThrow.Select(t => Vector3.Distance(startPosition, t.ColorTypeTransform.position)))
            {
                _maxDistance = Mathf.Max(_maxDistance, distance);
            }

            while (timeElapsed < MovementDuration)
            {
                IsMoving = true;
                for (var i = 0; i < objectsToThrow.Count; i++)
                {
                    var distance = Vector3.Distance(startPosition, objectsToThrow[i].ColorTypeTransform.position);
                    var duration = MovementDuration * (distance / _maxDistance);
                    _spheres[i].transform.position = Vector3.Lerp
                        (startPosition, objectsToThrow[i].ColorTypeTransform.position, timeElapsed / duration);
                }
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            IsMoving = false;
            

            foreach (var cube in cubes)
            {
                cube.MeshRenderer.enabled = false;
                cube.ParticleSystem.Play();
                cube.ParticleSystem.Play();
                cube.EmptyCubeLineRenderer.enabled = true;
            }

            foreach (var sphere in _spheres)
            {
                sphere.SetActive(false);
            }
            
            objectsToThrow.RemoveRange(0, objectsToThrow.Count);
            _spheres.RemoveRange(0, _spheres.Count);
        }
    }
}
