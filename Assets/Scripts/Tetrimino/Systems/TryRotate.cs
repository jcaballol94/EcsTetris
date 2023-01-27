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

            new TryRotateJob
            {
                rotation = GetRotation(input.value),
                localPosLookup = SystemAPI.GetComponentLookup<LocalBlock>(false)
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
        public int2x2 rotation;
        public int rotationInput;
        [NativeDisableParallelForRestriction] 
        public ComponentLookup<LocalBlock> localPosLookup;

        [BurstCompile]
        private void Execute(ref Orientation orientation, in DynamicBuffer<TetriminoBlockList> blocks)
        {
            foreach (var block in blocks)
            {
                var localPos = localPosLookup.GetRefRW(block.Value, false);
                localPos.ValueRW.position = math.mul(localPos.ValueRO.position, rotation);
            }

            orientation.value += rotationInput;
            if (orientation.value < 0)
                orientation.value += 4;
            if (orientation.value > 3)
                orientation.value -= 4;
        }
    }
}