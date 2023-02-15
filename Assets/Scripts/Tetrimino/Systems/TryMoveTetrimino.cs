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
    [UpdateInGroup(typeof(TetriminoMovementSystemGroup))]
    public partial struct TryMoveTetriminoSystem : ISystem
    {
        private EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAllRW<Transform>().WithAll<Rotation, ChildRef, MoveInput>().Build();

            state.RequireForUpdate(m_query);
            state.RequireForUpdate<GridBounds>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<GridBounds>(out var gridEntity))
                return;

            var gridAspect = SystemAPI.GetAspectRO<GridQueryAspect>(gridEntity);
            var blocksLookup = SystemAPI.GetComponentLookup<LocalPosition>(true);

            new TryMoveTetriminoJob
            {
                blocksLookup = blocksLookup,
                gridQuery = gridAspect
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct TryMoveTetriminoJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalPosition> blocksLookup;
        [ReadOnly][NativeDisableUnsafePtrRestriction] public GridQueryAspect gridQuery;

        [BurstCompile]
        private void Execute(ref Transform pos, in Rotation rot, in DynamicBuffer<ChildRef> children, in MoveInput input)
        {
            // Check if we have to move
            if (!input.changed || input.value == 0)
                return;

            // Calculate the new position
            var newPos = pos.position;
            newPos.x += input.value;

            var rotationMatrix = rot.GetMatrix();

            // Check that all the positions are available
            foreach (var child in children)
            {
                var childLocalPos = blocksLookup[child.value].value;
                if (!gridQuery.IsPositionAvailable(newPos + math.mul(childLocalPos, rotationMatrix)))
                    return;
            }

            // Apply the movement
            pos.position = newPos;
        }
    }
}