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
    public partial struct CleanupPlacedTetriminoSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(TetriminoTag))]
        public partial struct CleanupPlacedTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            [BurstCompile]
            private void Execute(Entity entity, in PlayerCleanupRef playerRef, in DynamicBuffer<TetriminoBlockBuffer> blocks)
            {
                // Set the blocks as static so they are taking into consideration for the collisions
                foreach (var block in blocks)
                {
                    ecb.AddComponent<StaticBlockTag>(block.value);
                }

                // Remove the reference to this tetrimino from the player, so it knows it has to spawn a new one
                ecb.RemoveComponent<TetriminoRef>(playerRef.value);

                // Remove the cleanup components to finish the deletion
                ecb.RemoveComponent<PlayerCleanupRef>(entity);
                ecb.RemoveComponent<TetriminoBlockBuffer>(entity);
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