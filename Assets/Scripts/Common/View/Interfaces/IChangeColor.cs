﻿using UnityEngine;

namespace Common.View
{
    public interface IChangeColor
    {
        ColorType ColorType { get; }
        MeshRenderer MeshRenderer { get; }

        ParticleSystem ParticleSystem { get; set; }
        
        LineRenderer EmptyCubeLineRenderer { get; set; }
        
        void SetColor(int type);

        void SetMaterial(ColorType colorType);
    }
}