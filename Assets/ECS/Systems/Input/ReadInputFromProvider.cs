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
        [WithChangeFilter(typeof(InputReader))]
        public partial struct ReadInputFromProviderJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<InputValues> valuesLookup;

            private void Execute(in InputReader reader, in DynamicBuffer<InputListener> listeners)
            {
                var readValues = new InputValues
                {
                    moveValue = reader.moveValue,
                    movePressed = reader.movePressed
                };
                var inputHash = readValues.GetHashCode();

                foreach (var listener in listeners)
                {
                    // Try only trigger the change filter when it has actually changed
                    var valuesRO = valuesLookup.GetRefRO(listener.value);
                    if (valuesRO.ValueRO.GetHashCode() != inputHash)
                    {
                        var values = valuesLookup.GetRefRW(listener.value, false);
                        values.ValueRW = readValues;
                    }
                }
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