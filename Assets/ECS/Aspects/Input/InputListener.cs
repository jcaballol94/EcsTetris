using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(1)]
    public struct InputListener : IBufferElementData
    {
        public Entity value;
    }
}
