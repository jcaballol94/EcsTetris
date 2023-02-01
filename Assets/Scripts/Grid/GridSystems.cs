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
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct GridToWorld : ISystem
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
            var gridDataLookup = SystemAPI.GetComponentLookup<GridToWorldData>(true);
            new GridToWorldJob()
            {
                gridDataLookup = gridDataLookup
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithChangeFilter(typeof(Position))]
    public partial struct GridToWorldJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<GridToWorldData> gridDataLookup;

        [BurstCompile]
        private void Execute(in GridReference grid, in Position posInGrid, ref TransformAspect transform)
        {
            var gridData = gridDataLookup[grid.value];

            transform.WorldPosition = gridData.origin + 
                (posInGrid.value.x + 0.5f) * gridData.right + 
                (posInGrid.value.y + 0.5f) * gridData.up;
        }
    }
}