using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [UpdateInGroup(typeof(PlayerMovement))]
    [UpdateAfter(typeof(ReadInput))]
    [RequireMatchingQueriesForUpdate]
    public partial struct TryMove : ISystem
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
            // Try to read input
            if (!SystemAPI.TryGetSingleton(out MoveInput input))
                return;

            // Check if we have to move
            if (!input.changed || input.value == 0)
                return;

            if (!SystemAPI.TryGetSingleton(out GridSize gridSize))
                return;

            new TryMoveJob
            {
                gridSize = gridSize.value,
                movementInput = input.value,
                localPosLookup = SystemAPI.GetComponentLookup<LocalBlock>(false),
            }.ScheduleParallel();
        }

        private static int2x2 GetRotation(int rotation)
        {
            if (rotation == 1)
            {
                return new int2x2(0, -1, 1, 0);
            }
            else if (rotation == -1)
            {
                return new int2x2(0, 1, -1, 0);
            }
            return int2x2.identity;
        }
    }

    [BurstCompile]
    public partial struct TryMoveJob : IJobEntity
    {
        public int2 gridSize;
        public int movementInput;
        [NativeDisableParallelForRestriction] 
        public ComponentLookup<LocalBlock> localPosLookup;

        [BurstCompile]
        private void Execute(ref Orientation orientation, ref PositionInGrid parentPos, in Tetrimino tetrimino,
            in DynamicBuffer<TetriminoBlockList> blocks)
        {
            var newTarget = parentPos.value;
            newTarget.x += movementInput;
            for (int i = 0; i < blocks.Length; ++i)
            {
                var localPos = localPosLookup.GetRefRO(blocks[i].Value);
                var futurePos = newTarget + localPos.ValueRO.position;
                if (futurePos.x < 0 || futurePos.y < 0 || futurePos.x >= gridSize.x || futurePos.y >= gridSize.y)
                    return;
            }

            parentPos.value = newTarget;
        }
    }
}