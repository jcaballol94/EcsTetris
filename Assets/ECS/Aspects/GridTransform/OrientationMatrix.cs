using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct OrientationMatrix : IComponentData
    {
        public int2x2 value;
    }
}