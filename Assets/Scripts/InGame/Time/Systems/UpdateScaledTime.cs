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
            state.EntityManager.AddComponent<TimeScale>(state.SystemHandle);
            state.EntityManager.SetComponentData(state.SystemHandle, new TimeScale { value = 1f });
            state.EntityManager.AddComponent<ScaledDeltaTime>(state.SystemHandle);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var scale = SystemAPI.GetComponent<TimeScale>(state.SystemHandle);
            // Update the time for the game objects as well
            Time.timeScale = scale.value;

            // Update the delta time
            var deltaTime = SystemAPI.Time.DeltaTime;
            SystemAPI.SetComponent(state.SystemHandle, new ScaledDeltaTime { value = deltaTime * scale.value });
        }
    }
}