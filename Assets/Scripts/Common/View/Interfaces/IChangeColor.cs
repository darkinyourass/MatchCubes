using UnityEngine;

namespace Common.View
{
    public interface IChangeColor
    {
        ColorType ColorType { get; set; }
        MeshRenderer MeshRenderer { get; set; }

        void SetColor(int type);

        void SetMaterial(ColorType colorType);

        void SetCurrentSelectableMaterial();
    }
}