using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct GridToWorldSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<GridToWorldData>(out var gridEntity))
                return;
            var grid = SystemAPI.GetAspectRO<GridAspect>(gridEntity);

            new GridToWorldJob
            {
                blockSize = grid.BlockSize,
                bottomLeft = grid.BottomLeft
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GridToWorldJob : IJobEntity
    {
        public float blockSize;
        public float3 bottomLeft;

        [BurstCompile]
        private void Execute([WithChangeFilter] in PositionInGrid pos, ref TransformAspect transform)
        {
            transform.WorldPosition = bottomLeft + new float3(blockSize * pos.value.x, blockSize * pos.value.y, 0);
        }
    }
}
