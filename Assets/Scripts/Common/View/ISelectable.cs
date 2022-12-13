using UnityEngine;

namespace Common.View
{
    public interface ISelectable
    {
        ColorType ColorType { get; set; }
        bool IsSelected { get; set; }
        
        Transform ColorTypeTransform { get; set; }
        void Select();
    }
}