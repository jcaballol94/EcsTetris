using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AfterInitialCommandBufferSystemGroup))]
    [UpdateAfter(typeof(SpawnTetriminoSystem))]
    public partial struct SpawnTetriminoBlocksSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Get all the tetriminos without blocks
            foreach (var (blockPrefab, tetriminoType, entity)
                in SystemAPI.Query<RefRO<BlockPrefab>, RefRO<TetriminoType>>()
                .WithNone<ChildBlockBuffer>()
                .WithEntityAccess())
            {
                ref var tetriminoData = ref tetriminoType.ValueRO.asset.Value;

                var blockBuffer = ecb.AddBuffer<ChildBlockBuffer>(entity);

                for (int i = 0; i < tetriminoData.blocks.Length; ++i)
                {
                    var blockEntity = ecb.Instantiate(blockPrefab.ValueRO.value);
                    ecb.SetName(blockEntity, "Block");
                    ecb.AddComponent(blockEntity, new ParentTetrimino { value = entity });
                    ecb.AddComponent(blockEntity, new LocalPosition { value = tetriminoData.blocks[i] });
                    ecb.AddComponent(blockEntity, new URPMaterialPropertyBaseColor { Value = tetriminoData.color });

                    blockBuffer.Add(new ChildBlockBuffer { value = blockEntity });
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}