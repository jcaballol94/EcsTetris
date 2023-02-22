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

            foreach (var (bounds, entity) in SystemAPI
                .Query<RefRO<GridBounds>>()
                .WithAll<GridWithCollisionsTag>()
                .WithNone<GridCellData>()
                .WithEntityAccess())
            {
                var size = bounds.ValueRO.size.x * bounds.ValueRO.size.y;

                var buffer = ecb.AddBuffer<GridCellData>(entity);
                buffer.Length = size;
                for (int i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] = GridCellData.Empty;
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}