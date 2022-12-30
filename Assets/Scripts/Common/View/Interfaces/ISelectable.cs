using UnityEngine;

namespace Common.View
{
    public interface ISelectable: IChangeColor
    {
        bool IsSelected { get; }
        Transform ColorTypeTransform { get; set; }
        void Select();

        int SelectingAnimationHash { get; set; }
        
        Animator Animator { get; set; }

    }
}