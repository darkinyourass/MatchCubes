using UnityEngine;

namespace Common.View
{
    public interface IChangeColor
    {
        ColorType ColorType { get; }
        MeshRenderer MeshRenderer { get; }

        void SetColor(int type);

        void SetMaterial(ColorType colorType);
    }
}