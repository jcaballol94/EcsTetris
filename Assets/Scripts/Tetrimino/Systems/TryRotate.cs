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
    public partial struct TryRotate : ISystem
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
            if (!SystemAPI.TryGetSingleton(out RotateInput input))
                return;

            // Check if we have to move
            if (!input.changed || input.value == 0)
                return;

            if (!SystemAPI.TryGetSingleton(out GridSize gridSize))
                return;

            new TryRotateJob
            {
                gridSize = gridSize.value,
                rotationInput = input.value,
                rotation = GetRotation(input.value),
                localPosLookup = SystemAPI.GetComponentLookup<LocalBlock>(false),
                offsetLookup = SystemAPI.GetBufferLookup<TetriminoOffsetDefinition>(true)
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
    public partial struct TryRotateJob : IJobEntity
    {
        public int2 gridSize;
        public int2x2 rotation;
        public int rotationInput;
        [NativeDisableParallelForRestriction] 
        public ComponentLookup<LocalBlock> localPosLookup;
        [ReadOnly] public BufferLookup<TetriminoOffsetDefinition> offsetLookup;

        [BurstCompile]
        private void Execute(ref Orientation orientation, ref PositionInGrid parentPos, in Tetrimino tetrimino,
            in DynamicBuffer<TetriminoBlockList> blocks)
        {
            offsetLookup.TryGetBuffer(tetrimino.definition, out var offsets);

            var newOrientation = orientation.value + rotationInput;
            if (newOrientation < 0)
                newOrientation += 4;
            if (newOrientation > 3)
                newOrientation -= 4;

            var canRotate = false;
            int step = 0;
            for (; !canRotate && step < 5; ++step)
            {
                canRotate = true;
                for (int j = 0; canRotate && j < blocks.Length; ++j)
                {
                    var initialOffset = offsets[orientation.value * 5 + step];
                    var targetOffset = offsets[newOrientation * 5 + step];

                    var localPos = localPosLookup.GetRefRO(blocks[j].Value);
                    var futurePos = parentPos.value + math.mul(localPos.ValueRO.position, rotation) + (initialOffset.value - targetOffset.value);
                    if (futurePos.x < 0 || futurePos.y < 0 || futurePos.x >= gridSize.x || futurePos.y >= gridSize.y)
                        canRotate = false;
                }
            }
            step--; // When we exit the loop, the step will have increased

            if (!canRotate)
                return;

            foreach (var block in blocks)
            {
                var localPos = localPosLookup.GetRefRW(block.Value, false);
                localPos.ValueRW.position = math.mul(localPos.ValueRO.position, rotation);
            }
            var stepInitial = offsets[orientation.value * 5 + step];
            var stepFinal = offsets[newOrientation * 5 + step];
            parentPos.value += (stepInitial.value - stepFinal.value);
            orientation.value = newOrientation;
        }
    }
}