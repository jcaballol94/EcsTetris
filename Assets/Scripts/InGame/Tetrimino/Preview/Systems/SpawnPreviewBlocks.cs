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
    public partial struct SpawnPreviewBlocksSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(PreviewBlocks))]
        [WithAll(typeof(PreviewTag))]
        public partial struct SpawnPreviewBlocksJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public GameSkin skin;

            [BurstCompile]
            private void Execute(Entity entity, in GridRef grid)
            {
                var blocks = ecb.AddBuffer<PreviewBlocks>(entity);
                for (int i = 0; i < 4; ++i)
                {
                    var block = ecb.Instantiate(skin.blockPrefab);
                    ecb.SetName(block, "PreviewBlock");
                    ecb.AddComponent(block, grid);
                    ecb.AddComponent<BlockPosition>(block);
                    blocks.Add(new PreviewBlocks { value = block });
                }
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameSkin>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton<GameSkin>(out var skin)) return;

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

            new SpawnPreviewBlocksJob
            {
                skin = skin,
                ecb = ecb
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}