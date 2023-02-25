using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial struct WaitForSpawnDelaySystem : ISystem
    {
        [BurstCompile]
        public partial struct WaitForSpawnDelayJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public float deltaTime;

            [BurstCompile]
            private void Execute(Entity entity, ref SpawnTetriminoDelay delay)
            {
                delay.value -= deltaTime;
                if (delay.value <= 0f)
                    ecb.RemoveComponent<SpawnTetriminoDelay>(entity);
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScaledDeltaTime>();
            state.RequireForUpdate<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out ScaledDeltaTime deltaTime)) return;
            if (!SystemAPI.TryGetSingleton(out EndVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            new WaitForSpawnDelayJob
            {
                deltaTime = deltaTime.value,
                ecb = ecb
            }.Schedule();
        }
    }
}