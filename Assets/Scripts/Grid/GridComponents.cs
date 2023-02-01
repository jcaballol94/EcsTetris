using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct GridReference : IComponentData
    {
        public Entity value;
    }

    public struct Position : IComponentData
    {
        public int2 value;
    }

    public struct GridToWorldData : IComponentData
    {
        public float3 origin;
        public float3 up;
        public float3 right;
        public float blockSize;
    }
}