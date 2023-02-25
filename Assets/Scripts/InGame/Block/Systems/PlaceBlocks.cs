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
    [UpdateBefore(typeof(CleanupTetriminoSystem))]
    public partial struct PlaceBlocksSystem : ISystem
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

            foreach (var (blocks, entity) in SystemAPI
                .Query<DynamicBuffer<TetriminoBlockBuffer>>()
                .WithAll<StaticBlockTag>()
                .WithEntityAccess())
            {
                foreach (var block in blocks)
                {
                    // Set the blocks as static
                    ecb.AddComponent<StaticBlockTag>(block.value);
                }
                // Unlink the blocks
                blocks.Clear();
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}