using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct GridAspect : IAspect
    {
        public readonly Entity Self;

        readonly TransformAspect transform;
        readonly RefRO<GridSize> size;
        readonly RefRO<GridToWorldData> worldData;

        public float3 BottomLeft => transform.WorldPosition + new float3(BlockSize * 0.5f, BlockSize * 0.5f, 0);
        public int2 Size => size.ValueRO.value;
        public float BlockSize => worldData.ValueRO.blockSize;
    }
}
