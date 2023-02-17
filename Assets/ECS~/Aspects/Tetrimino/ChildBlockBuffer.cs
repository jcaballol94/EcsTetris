using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(4)]
    public struct ChildBlockBuffer : IBufferElementData
    {
        public Entity value;
    }
}
