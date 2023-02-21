using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct PlaceTetriminoEvent : IBufferElementData
    {
        public Entity tetrimino;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct PlaceTetriminoEventSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.AddComponent<PlaceTetriminoEvent>(state.SystemHandle);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = state.EntityManager.GetBuffer<PlaceTetriminoEvent>(state.SystemHandle);
            buffer.Clear();
        }
    }
}