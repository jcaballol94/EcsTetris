using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct ParentGridTransform : IComponentData
    {
        public int2 position;
        public int2x2 matrix;

        public int2 TransformPoint(in int2 point)
        {
            return position + math.mul(point, matrix);
        }
    }
}