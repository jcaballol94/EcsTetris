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
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial struct AddComponentsToGameGridSystem : ISystem
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
            foreach (var playerData in SystemAPI.Query<RefRO<PlayerData>>())
            {
                var grid = playerData.ValueRO.mainGrid;
                ecb.AddComponent<GridWithCollisionsTag>(grid);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}