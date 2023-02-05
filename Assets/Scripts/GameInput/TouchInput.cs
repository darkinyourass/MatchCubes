using System;
using Common.View;
using UnityEngine;

namespace GameInput
{
    public class TouchInput: MonoBehaviour
    {
        public event Action OnMouseDownOverCube;
        public event Action OnTouchOrMouseUp;
        
        private Touch _touch;

        private void Update()
        {
            SelectCubes();
            DeselectCubes();
            MoveWithTouch();
        }

        private void SelectCubes()
        {

            if (Input.GetMouseButton(0))
            {
                OnMouseDownOverCube?.Invoke();
            }
        }

        private void DeselectCubes()
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnTouchOrMouseUp?.Invoke();
            }
        }

        private void MoveWithTouch()
        {
            if (Input.touchCount != 1) return;
            _touch = Input.GetTouch(0);
            
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    OnMouseDownOverCube?.Invoke();
                    break;
                case TouchPhase.Moved:
                    OnMouseDownOverCube?.Invoke();
                    break;
                case TouchPhase.Ended:
                    OnTouchOrMouseUp?.Invoke();
                    break;
            }
        }
    }
}