using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Selection : MonoBehaviour
    {
        [field: SerializeField]
        public LineRenderer SelectionRenderer { get; set; }

        private void Awake()
        {
            SelectionRenderer = GetComponent<LineRenderer>();
        }
    }
}