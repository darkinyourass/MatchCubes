using System;
using UnityEngine;

namespace Common.View
{
    public interface ISelectable: IChangeColor
    {
        bool IsSelected { get; }
        Transform ColorTypeTransform { get; set; }
        void Select();

        event Action<ISelectable> OnMouseDownAsButton;
        
        int SelectingAnimationHash { get; set; }
        
        Animator Animator { get; set; }

    }
}