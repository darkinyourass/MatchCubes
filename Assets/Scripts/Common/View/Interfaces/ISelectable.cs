using UnityEngine;

namespace Common.View
{
    public interface ISelectable: IChangeColor
    {
        bool IsSelected { get; }
        Transform ColorTypeTransform { get; set; }
        void Select();

    }
}