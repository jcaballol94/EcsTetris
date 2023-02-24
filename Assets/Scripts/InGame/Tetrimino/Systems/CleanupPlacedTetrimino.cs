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
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct CleanupPlacedTetriminoSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(TetriminoTag))]
        public partial struct CleanupPlacedTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            [BurstCompile]
            private void Execute(Entity entity, in PlayerCleanupRef playerRef)
            {
                ecb.RemoveComponent<TetriminoRef>(playerRef.value);
                ecb.RemoveComponent<PlayerCleanupRef>(entity);
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
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

            new CleanupPlacedTetriminoJob()
            {
                ecb = ecb
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}