using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GridTransformSystemGroup), OrderLast = true)]
    public partial struct TransformFromGridSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter]
        public partial struct TransformFromGridJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<GridTransformData> transformDataLookup;

            [BurstCompile]
            private void Execute(in WorldGridTransform pos, ref LocalTransform transform, in GridRef grid)
            {
                var transformData = transformDataLookup[grid.value];

                transform.Position = transformData.origin
                    + ((pos.position.x + 0.5f) * transformData.right + (pos.position.y + 0.5f) * transformData.up)
                    * transformData.scale;
            }
        }

        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var gridDataLookup = SystemAPI.GetComponentLookup<GridTransformData>(true);

            new TransformFromGridJob
            {
                transformDataLookup = gridDataLookup,
            }.ScheduleParallel();
        }
    }
}