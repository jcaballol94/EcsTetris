using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
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
            public GridTransformData transformData;

            [BurstCompile]
            private void Execute(in WorldGridPosition pos, ref LocalTransform transform)
            {
                transform.Position = transformData.origin
                    + (pos.value.x * transformData.right + pos.value.y * transformData.up)
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
            // Build the query
            var query = SystemAPI.QueryBuilder()
                .WithAllRW<LocalTransform>()
                .WithAll<WorldGridPosition, GridRef>()
                .Build();
            

            // Get all the available grids
            state.EntityManager.GetAllUniqueSharedComponents<GridRef>(out var grids, Unity.Collections.Allocator.Temp);
            foreach (var grid in grids)
            {
                // It will always return the default value, which is a null entity
                if (grid.value == Entity.Null) continue;

                query.ResetFilter();
                query.AddChangedVersionFilter(typeof(WorldGridPosition));
                query.AddSharedComponentFilter(grid);

                new TransformFromGridJob
                {
                    transformData = state.EntityManager.GetComponentData<GridTransformData>(grid.value)
                }.ScheduleParallel(query);
            }

            grids.Dispose();
        }
    }
}