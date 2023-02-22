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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(TetriminoBlockTransformSystem))]
    public partial struct BlockTransformSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter]
        public partial struct BlockTransformJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<GridTransformData> transformDataLookup;

            [BurstCompile]
            private void Execute(in BlockPosition pos, ref LocalTransform transform, in GridRef grid)
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

            new BlockTransformJob
            {
                transformDataLookup = gridDataLookup,
            }.ScheduleParallel();
        }
    }
}