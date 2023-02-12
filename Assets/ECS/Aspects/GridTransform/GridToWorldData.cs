using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct GridToWorldData : IComponentData
    {
        public float3 origin;
        public float3 up;
        public float3 right;
        public float blockSize;
    }
}