using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(20)]
    public struct TetriminoBlockDefinition : IBufferElementData
    {
        public int2 Value;
    }
}
