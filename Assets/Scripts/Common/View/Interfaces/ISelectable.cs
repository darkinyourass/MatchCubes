using System;
using UnityEngine;

namespace Common.View
{
    public interface ISelectable: IChangeColor
    {
        bool IsSelected { get; }
        Transform ColorTypeTransform { get; set; }
        void SelectDeselect();

        event Action<ISelectable> OnMouseDownAsButton;
        
        // public event Action<ISelectable> OnMouseDownOverCube;
        //
        // public event Action OnTouchOrMouseUp;
        
        int SelectingAnimationHash { get; set; }
        
        int SpawningAnimationHash { get; set; }
        
        Animator Animator { get; set; }
        
        LineRenderer SelectedCubeLineRenderer { get; set; }

    }
}