using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
            foreach (var (gameModeDataRef, tetriminoType, blockPrefab, entity)
                in SystemAPI.Query<RefRO<GameModeData>, RefRO<TetriminoType>, RefRO<BlockPrefab>>()
                .WithNone<ChildBlockBuffer>()
                .WithEntityAccess())
            {
                ref var tetriminoData = ref gameModeDataRef.ValueRO.GetTetriminoDefinition(tetriminoType.ValueRO);

                var blockBuffer = ecb.AddBuffer<ChildBlockBuffer>(entity);

                for (int i = 0; i < tetriminoData.blocks.Length; ++i)
                {
                    var blockEntity = ecb.Instantiate(blockPrefab.ValueRO.value);
                    blockBuffer.Add(new ChildBlockBuffer { value = blockEntity });
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}