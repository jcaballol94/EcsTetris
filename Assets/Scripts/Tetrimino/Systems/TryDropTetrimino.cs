using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(SpawnTetriminoSystem))]
    public partial struct IAddDropStateToTetrimino : ISystem
    {
        private EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<TetriminoTag>().WithNone<TetriminoDropState>().Build();
            state.RequireForUpdate(m_query);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<TetriminoDropState>(m_query);
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(TetriminoMovementSystemGroup))]
    public partial struct TryDropTetrimino : ISystem
    {
        private EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAllRW<Position, TetriminoDropState>().WithAll<Rotation, ChildRef, DropInput>().Build();

            state.RequireForUpdate(m_query);
            state.RequireForUpdate<GridBounds>();
            state.RequireForUpdate<DropData>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            if (!SystemAPI.TryGetSingletonEntity<GridBounds>(out var gridEntity))
                return;

            if (!SystemAPI.TryGetSingleton<DropData>(out var data))
                return;

            var gridAspect = SystemAPI.GetAspectRO<GridQueryAspect>(gridEntity);
            var blocksLookup = SystemAPI.GetComponentLookup<LocalPosition>(true);

            new TryDropTetriminoJob
            {
                blocksLookup = blocksLookup,
                gridQuery = gridAspect,
                data = data,
                deltaTime = deltaTime
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct TryDropTetriminoJob : IJobEntity
    {
        public float deltaTime;
        public DropData data;
        [ReadOnly][NativeDisableUnsafePtrRestriction] public GridQueryAspect gridQuery;
        [ReadOnly] public ComponentLookup<LocalPosition> blocksLookup;

        [BurstCompile]
        private void Execute(ref TetriminoDropState state, in DropInput input, ref Position pos, in Rotation rot, in DynamicBuffer<ChildRef> blocks)
        {
            state.timeSinceLastDrop += deltaTime;
            var speed = data.dropSpeed;
            if (input.fast)
                speed *= data.fastDropMultiplier;

            // Check that it needs to fall
            if ((1f / speed) > state.timeSinceLastDrop)
                return;

            state.timeSinceLastDrop = 0;

            var rotationMatrix = rot.GetMatrix();
            var newPos = pos.value;
            newPos.y -= 1;

            // Check if the blocks are touching anything
            foreach (var block in blocks)
            {
                var localPos = blocksLookup[block.value].value;
                var blockPos = newPos + math.mul(localPos, rotationMatrix);

                if (!gridQuery.IsPositionAvailable(blockPos))
                    return;
            }

            // Apply the new position
            pos.value = newPos;
        }
    }
}