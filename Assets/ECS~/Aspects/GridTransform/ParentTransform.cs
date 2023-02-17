using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct ParentTransform : IComponentData
    {
        public int2 position;
        public int2x2 matrix;
    }
}