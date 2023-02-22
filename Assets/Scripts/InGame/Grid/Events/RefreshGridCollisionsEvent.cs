using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct RefreshGridCollisionsEvent : IBufferElementData
    {
        public Entity grid;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct RefereshGridCollisionsEventSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.AddComponent<RefreshGridCollisionsEvent>(state.SystemHandle);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = state.EntityManager.GetBuffer<RefreshGridCollisionsEvent>(state.SystemHandle);
            buffer.Clear();
        }
    }
}