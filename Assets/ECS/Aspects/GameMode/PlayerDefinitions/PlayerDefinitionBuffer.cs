using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(0)]
    public struct PlayerDefinitionBuffer : IBufferElementData
    {
        public int value;
    }
}
