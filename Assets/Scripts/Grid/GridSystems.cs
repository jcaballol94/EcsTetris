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
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class GridToWorldSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GridToWorldSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct AddPositionComponents : ISystem
    {
        private EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<GridReference, LocalPosition, TetriminoRef>().WithNone<Position>().Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<Position>(m_query);
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(GridToWorldSystemGroup))]
    [UpdateAfter(typeof(AddPositionComponents))]
    [UpdateBefore(typeof(GridToWorld))]
    [RequireMatchingQueriesForUpdate]
    public partial struct LocalToPosSystem : ISystem
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
            var positionLookup = SystemAPI.GetComponentLookup<Position>(false);

            new LocalToPosJob
            {
                positionLookup = positionLookup
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct LocalToPosJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<Position> positionLookup;

        [BurstCompile]
        public void Execute(Entity block, in LocalPosition local, in TetriminoRef tetrimino)
        {
            var parentPos = positionLookup.GetRefRO(tetrimino.value);
            var position = positionLookup.GetRefRW(block, false);

            position.ValueRW.value = parentPos.ValueRO.value + local.value;
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(GridToWorldSystemGroup))]
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