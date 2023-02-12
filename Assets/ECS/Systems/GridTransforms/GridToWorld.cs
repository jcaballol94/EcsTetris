using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GridTransformsSystemGroup), OrderLast = true)]
    public partial struct GridToWorldSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(Position))]
        public partial struct GridToWorldJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in Position pos, in GridToWorldData data, ref Unity.Transforms.LocalTransform transform)
            {
                var offset = pos.value.x * data.right + pos.value.y * data.up;
                transform.Position = data.origin + new float3(0.5f, 0.5f, 0f) + offset * data.blockSize;
            }
        }

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
            new GridToWorldJob().ScheduleParallel();
        }
    }
}