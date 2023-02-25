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
    [UpdateAfter(typeof(SpawnTetriminoSystem))]
    public partial struct SpawnBlocksSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(TetriminoBlockBuffer))]
        [WithAll(typeof(TetriminoTag))]
        public partial struct SpawnBlocksJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            public GameSkin skin;

            [BurstCompile]
            private void Execute(Entity entity, TetriminoData data, in GridRef gridRef)
            {
                var buffer = ecb.AddBuffer<TetriminoBlockBuffer>(entity);

                ref var blocks = ref data.blocks;

                for (int i = 0; i < blocks.Length; ++i)
                {
                    var block = ecb.Instantiate(skin.blockPrefab);
                    ecb.SetName(block, "Block");

                    buffer.Add(new TetriminoBlockBuffer { value = block }); ;

                    ecb.AddComponent(block, new BlockPosition { position = blocks[i] });
                    ecb.SetComponent(block, new Unity.Rendering.URPMaterialPropertyBaseColor { Value = data.color });
                    ecb.AddComponent(block, gridRef);
                }
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameSkin>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out EndInitializationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            if (!SystemAPI.TryGetSingleton(out GameSkin gameSkin)) return;

            new SpawnBlocksJob
            {
                ecb = ecb,
                skin = gameSkin
            }.Schedule();
        }
    }
}