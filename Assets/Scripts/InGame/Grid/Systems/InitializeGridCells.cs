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
    public partial struct InitializeGridCellsSystem : ISystem
    {
        [WithAll(typeof(GridWithCollisionsTag))]
        [WithNone(typeof(GridCellData))]
        [BurstCompile]
        public partial struct InitializeGridCellsJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            [BurstCompile]
            private void Execute(Entity entity, in GridBounds bounds)
            {
                var size = bounds.size.x * bounds.size.y;

                var buffer = ecb.AddBuffer<GridCellData>(entity);
                buffer.Length = size;
                for (int i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] = GridCellData.Empty;
                }
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
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

            new InitializeGridCellsJob
            {
                ecb = ecb
            }.Schedule();
        }
    }
}