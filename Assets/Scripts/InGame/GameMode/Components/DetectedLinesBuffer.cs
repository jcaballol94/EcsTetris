using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(4)]
    public struct DetectedLinesBuffer : IBufferElementData
    {
        public int lineY;
    }
}