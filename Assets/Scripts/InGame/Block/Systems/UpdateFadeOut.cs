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
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial struct UpdateFadeOutSystem : ISystem
    {
        [BurstCompile]
        public partial struct UpdateFadeOutJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public float delta;

            [BurstCompile]
            private void Execute(ref FadeOut fade, in GridRef grid)
            {
                fade.Value += delta;

                if (fade.Value >= 1f)
                    ecb.RemoveComponent<WaitForAnimation>(grid.value);
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<ScaledDeltaTime>();
            state.RequireForUpdate<DetectedLinesBuffer>();

            state.RequireForUpdate<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out EndVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;
            if (!SystemAPI.TryGetSingleton(out ScaledDeltaTime deltaTime)) return;

            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            var delta = deltaTime.value / gameData.removeLineDuration;
            new UpdateFadeOutJob
            {
                delta = delta,
                ecb = ecb
            }.Schedule();
        }
    }
}