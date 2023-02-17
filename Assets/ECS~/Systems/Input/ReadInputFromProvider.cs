using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(ReadInputFromManagedSystem))]
    public partial struct ReadInputFromProviderSystem : ISystem
    {
        [WithChangeFilter(typeof(UnmanagedInput))]
        public partial struct ReadInputFromProviderJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<InputValues> valuesLookup;

            private void Execute(in UnmanagedInput reader, in CurrentTetrimino tetrimino)
            {
                var readValues = new InputValues
                {
                    moveValue = reader.moveValue,
                    movePressed = reader.movePressed,
                    rotateValue = reader.rotateValue,
                    rotatePressed = reader.rotatePressed,
                    fallFast = reader.fallFast,
                    drop = reader.drop && !reader.prevDrop
                };

                var values = valuesLookup.GetRefRW(tetrimino.value, false);
                values.ValueRW = readValues;
            }
        }

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
            var valuesLookup = SystemAPI.GetComponentLookup<InputValues>(false);
            new ReadInputFromProviderJob
            {
                valuesLookup = valuesLookup
            }.ScheduleParallel();
        }
    }
}