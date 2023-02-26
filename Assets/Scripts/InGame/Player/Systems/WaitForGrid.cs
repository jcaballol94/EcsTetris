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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InitializeFadeOutSystem))]
    [UpdateBefore(typeof(SpawnTetriminoSystem))]
    public partial struct WaitForGridSystem : ISystem
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
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var player in SystemAPI
                .Query<RefRO<PlayerRef>>()
                .WithAll<WaitForAnimation>())
            {
                ecb.AddComponent<WaitForAnimation>(player.ValueRO.value);
            }

            foreach (var player in SystemAPI
                .Query<RefRO<PlayerRef>>()
                .WithNone<WaitForAnimation>())
            {
                ecb.RemoveComponent<WaitForAnimation>(player.ValueRO.value);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}