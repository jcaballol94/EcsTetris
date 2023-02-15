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
            m_query = SystemAPI.QueryBuilder().WithAll<GridReference, LocalPosition, TetriminoRef>().WithNone<Transform>().Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<Transform>(m_query);
        }
    }
    [UpdateInGroup(typeof(GridToWorldSystemGroup))]
    [BurstCompile]
    public partial struct AddRotationMatrixComponents : ISystem
    {
        private EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<Rotation>().WithNone<RotationMatrix>().Build();

            state.RequireForUpdate(m_query);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<RotationMatrix>(m_query);
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(GridToWorldSystemGroup))]
    [UpdateAfter(typeof(AddRotationMatrixComponents))]
    [UpdateBefore(typeof(GridToWorld))]
    [RequireMatchingQueriesForUpdate]
    public partial struct FillRotationMatrix : ISystem
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
            new UpdateRotationMatrixJob().ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithChangeFilter(typeof(Rotation))]
    public partial struct UpdateRotationMatrixJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(in Rotation rotation, ref RotationMatrix matrix)
        {
            matrix.value = rotation.GetMatrix();
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(GridToWorldSystemGroup))]
    [UpdateAfter(typeof(AddPositionComponents))]
    [UpdateAfter(typeof(FillRotationMatrix))]
    [UpdateAfter(typeof(AddRotationMatrixComponents))]
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
            var positionLookup = SystemAPI.GetComponentLookup<Transform>(false);
            var rotationLookup = SystemAPI.GetComponentLookup<RotationMatrix>(true);

            new LocalToPosJob
            {
                positionLookup = positionLookup,
                rotationLookup = rotationLookup
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct LocalToPosJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<RotationMatrix> rotationLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<Transform> positionLookup;

        [BurstCompile]
        public void Execute(Entity block, in LocalPosition local, in TetriminoRef tetrimino)
        {
            var parentPos = positionLookup.GetRefRO(tetrimino.value);
            var position = positionLookup.GetRefRW(block, false);
            var rotation = rotationLookup.GetRefRO(tetrimino.value);

            position.ValueRW.position = parentPos.ValueRO.position + math.mul(local.value, rotation.ValueRO.value);
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
    [WithChangeFilter(typeof(Transform))]
    public partial struct GridToWorldJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<GridToWorldData> gridDataLookup;

        [BurstCompile]
        private void Execute(in GridReference grid, in Transform posInGrid, ref TransformAspect transform)
        {
            var gridData = gridDataLookup[grid.value];

            transform.WorldPosition = gridData.origin + 
                (posInGrid.position.x + 0.5f) * gridData.right + 
                (posInGrid.position.y + 0.5f) * gridData.up;
        }
    }
}