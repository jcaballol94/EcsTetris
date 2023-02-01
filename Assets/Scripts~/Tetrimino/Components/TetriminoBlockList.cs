using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(4)]
    public struct TetriminoBlockList : IBufferElementData
    {
        public Entity Value;
    }
}
