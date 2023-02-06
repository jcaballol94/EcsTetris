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
    public partial struct TryRotateTetriminoSystem : ISystem
    {
        private EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAllRW<Position, Rotation>()
                .WithAll<ChildRef, RotateInput, TetriminoDefinitionRef>().Build();

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
            var offsetLookup = SystemAPI.GetBufferLookup<TetriminoRotationOffsets>(true);

            new TryRotateTetriminoJob
            {
                blocksLookup = blocksLookup,
                offsetLookup = offsetLookup,
                gridQuery = gridAspect
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct TryRotateTetriminoJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalPosition> blocksLookup;
        [ReadOnly] public BufferLookup<TetriminoRotationOffsets> offsetLookup;
        [ReadOnly][NativeDisableUnsafePtrRestriction] public GridQueryAspect gridQuery;

        [BurstCompile]
        private void Execute(ref Position pos, ref Rotation rot, in DynamicBuffer<ChildRef> children, 
            in RotateInput input, in TetriminoDefinitionRef definition)
        {
            // Check if we have to move
            if (!input.changed || input.value == 0)
                return;

            // Calculate the new rotation
            var newRotation = rot.GetRotated(input.value);
            var rotationMatrix = RotationMatrix.FromRotation(newRotation);

            var offsetBuffer = offsetLookup[definition.value];

            // Check all the offsets in order
            bool canRotate = false;
            int step = 0;
            int2 offset = int2.zero;
            for (; !canRotate && step < 5; ++step)
            {
                canRotate = true;

                var initialOffset = offsetBuffer[rot.value * 5 + step];
                var targetOffset = offsetBuffer[newRotation * 5 + step];

                offset = initialOffset.value - targetOffset.value;

                // Check that all the positions are available
                foreach (var child in children)
                {
                    var childLocalPos = blocksLookup[child.value].value;
                    if (!gridQuery.IsPositionAvailable(pos.value + offset + math.mul(childLocalPos, rotationMatrix)))
                    {
                        canRotate = false;
                        break;
                    }
                }
            }

            // If the rotation was possible, apply the rotation
            if (canRotate)
            {
                pos.value += offset;
                rot.value = newRotation;
            }
        }
    }
}