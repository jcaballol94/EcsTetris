using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct RequestSpawnTetriminoEvent : IBufferElementData
    {
        public Entity player;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct RequestSpawnTetriminoEventSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.AddComponent<RequestSpawnTetriminoEvent>(state.SystemHandle);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = state.EntityManager.GetBuffer<RequestSpawnTetriminoEvent>(state.SystemHandle);
            buffer.Clear();
        }
    }
}