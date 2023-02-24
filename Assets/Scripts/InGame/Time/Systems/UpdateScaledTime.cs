using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderFirst = true)]
    public partial struct UpdateScaledTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var rawDelta = SystemAPI.Time.DeltaTime;
            foreach (var (scale, delta) in SystemAPI.Query<RefRO<TimeScale>, RefRW<ScaledDeltaTime>>())
            {
                // Update the time for the game objects as well
                Time.timeScale = scale.ValueRO.value;

                // Update the delta time
                delta.ValueRW.value = math.min(rawDelta * scale.ValueRO.value, 0.1f);
            }
        }
    }
}