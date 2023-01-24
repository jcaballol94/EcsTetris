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
        EntityQuery query;

        public void OnCreate(ref SystemState state)
        {
            query = new EntityQueryBuilder(Unity.Collections.Allocator.Temp)
                .WithAll<LocalToWorld>()
                .WithAllRW<PositionInGrid>()
                .Build(ref state);
            query.AddChangedVersionFilter(typeof(PositionInGrid));
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
            }.ScheduleParallel(query);
        }
    }

    public partial struct GridToWorldJob : IJobEntity
    {
        public float blockSize;
        public float3 bottomLeft;

        private void Execute(in PositionInGrid pos, ref TransformAspect transform)
        {
            transform.WorldPosition = bottomLeft + new float3(blockSize * pos.value.x, blockSize * pos.value.y, 0);
        }
    }
}
