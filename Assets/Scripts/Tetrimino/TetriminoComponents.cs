using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(4)]
    public struct ChildRef : IBufferElementData
    {
        public Entity value;
    }

    public struct TetriminoDefinitionRef : IComponentData
    {
        public Entity value;
    }

    public struct TetriminoRef : IComponentData
    {
        public Entity value;
    }

    [InternalBufferCapacity(4)]
    public struct TetriminoBlockDefinition : IBufferElementData 
    {
        public int2 value;
    }

    [InternalBufferCapacity(20)]
    public struct TetriminoRotationOffsets : IBufferElementData 
    {
        public int2 value;
    }

    public struct TetriminoColorDefinition : IComponentData
    {
        public float4 value;
    }

    public struct TetriminoDropState : IComponentData
    {
        public float timeSinceLastDrop;
    }
}