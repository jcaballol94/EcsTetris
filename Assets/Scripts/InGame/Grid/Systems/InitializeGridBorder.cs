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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial struct InitializeGridBorderSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(GridHasBorderTag))]
        public partial struct InitializeGridBorderJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public GameSkin skin;

            [BurstCompile]
            private void Execute(Entity entity, in GridBounds bounds, in GridBorder borderData)
            {
                CreateHorizontalLine(new int2(-1, -1), new int2(bounds.size.x, -1), ecb, entity, skin.blockPrefab, borderData.color);
                CreateHorizontalLine(new int2(-1, bounds.size.y), bounds.size, ecb, entity, skin.blockPrefab, borderData.color);
                CreateVerticalLine(new int2(-1, 0), new int2(-1, bounds.size.y - 1), ecb, entity, skin.blockPrefab, borderData.color);
                CreateVerticalLine(new int2(bounds.size.x, 0), new int2(bounds.size.x, bounds.size.y - 1), ecb, entity, skin.blockPrefab, borderData.color);

                ecb.AddComponent<GridHasBorderTag>(entity);
            }

            private void CreateHorizontalLine(int2 start, int2 end, EntityCommandBuffer ecb, Entity grid, Entity blockPrefab, float4 color)
            {
                for (int i = start.x; i <= end.x; ++i)
                {
                    CreateBlock(new int2(i, start.y), ecb, grid, blockPrefab, color);
                }
            }

            private void CreateVerticalLine(int2 start, int2 end, EntityCommandBuffer ecb, Entity grid, Entity blockPrefab, float4 color)
            {
                for (int i = start.y; i <= end.y; ++i)
                {
                    CreateBlock(new int2(start.x, i), ecb, grid, blockPrefab, color);
                }
            }

            private void CreateBlock(int2 pos, EntityCommandBuffer ecb, Entity grid, Entity blockPrefab, float4 color)
            {
                var block = ecb.Instantiate(blockPrefab);
                ecb.AddComponent(block, new BlockPosition { position = pos });
                ecb.AddComponent(block, new GridRef { value = grid });
                ecb.SetComponent(block, new Unity.Rendering.URPMaterialPropertyBaseColor { Value = color });
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
            if (!SystemAPI.TryGetSingleton(out GameSkin gameSkin)) return;
            if (!SystemAPI.TryGetSingleton(out EndInitializationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            new InitializeGridBorderJob
            {
                ecb = ecb,
                skin = gameSkin
            }.Schedule();
        }
    }
}