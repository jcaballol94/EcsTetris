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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct CleanupTetriminoSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(TetriminoTag))]
        public partial struct CleanupTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            [ReadOnly] public ComponentLookup<BlockPosition> positionLookup;
            [ReadOnly] public ComponentLookup<TetriminoRef> tetriminoRefLookup;

            [BurstCompile]
            private void Execute(Entity entity, in PlayerCleanupRef playerRef, in DynamicBuffer<TetriminoBlockBuffer> blocks)
            {
                // Remove all the blocks
                foreach (var block in blocks)
                {
                    // Ensure that it is a valid block
                    if (positionLookup.HasComponent(block.value))
                        ecb.DestroyEntity(block.value);
                }

                // Remove the reference to this tetrimino from the player, so it knows it has to spawn a new one
                if (tetriminoRefLookup.HasComponent(playerRef.value))
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

            var positionLookup = SystemAPI.GetComponentLookup<BlockPosition>(true);
            var tetriminoRefLookup = SystemAPI.GetComponentLookup<TetriminoRef>(true);

            new CleanupTetriminoJob()
            {
                ecb = ecb,
                positionLookup = positionLookup,
                tetriminoRefLookup = tetriminoRefLookup
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}