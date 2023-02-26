using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Tetris
{
    [MaterialProperty("_Fade")]
    public struct FadeOut : IComponentData
    {
        public float Value;

        public static readonly FadeOut Start = new FadeOut { Value = 0f };
    }
}