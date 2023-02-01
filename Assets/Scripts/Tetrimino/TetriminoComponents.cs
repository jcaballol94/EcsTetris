using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct TetriminoTag : IComponentData { }

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

    public struct LocalPosition : IComponentData
    {
        public int2 value;
    }

    public struct TetriminoBlockDefinition : IBufferElementData 
    {
        public int2 value;
    }
}