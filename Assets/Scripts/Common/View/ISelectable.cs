using UnityEngine;

namespace Common.View
{
    public interface ISelectable
    {
        ColorType ColorType { get; set; }
        bool IsSelected { get; }

        Transform ColorTypeTransform { get; set; }

        MeshRenderer MeshRenderer { get; }

        void Select();

    }
}