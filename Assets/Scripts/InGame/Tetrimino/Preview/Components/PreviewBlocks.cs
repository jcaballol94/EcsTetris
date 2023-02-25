using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(4)]
    public struct PreviewBlocks : IBufferElementData
    {
        public Entity value;
    }
}