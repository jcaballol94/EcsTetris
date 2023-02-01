using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class SpawnSystemGroup : ComponentSystemGroup { }

    [BurstCompile]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    public partial struct SpawnTetriminoSystem : ISystem
    {
        private EntityQuery m_blocksQuery;
        private EntityQuery m_gameModeQuery;

        public void OnCreate(ref SystemState state)
        {
            m_blocksQuery = SystemAPI.QueryBuilder().WithAll<FallingBlockTag>().Build();
            m_gameModeQuery = SystemAPI.QueryBuilder().WithAll<TetrisGameMode>().Build();

            state.RequireForUpdate(m_gameModeQuery);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            // There are blocks, no need to spawn more
            if (m_blocksQuery.CalculateChunkCount() > 0)
                return;

            // Try to get the data required
            if (!m_gameModeQuery.TryGetSingleton<TetrisGameMode>(out var gameMode))
                return;

            var ecbSystem = state.World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer();

            var blockEntity = ecb.Instantiate(gameMode.blockPrefab);
            // Label as a falling block
            ecb.AddComponent<FallingBlockTag>(blockEntity);
            // Link a reference to the grid
            ecb.AddComponent(blockEntity, new GridReference { value = gameMode.mainGrid });
            // Put it in the right position
            ecb.AddComponent(blockEntity, new Position { value = gameMode.spawnPosition });
        }
    }
}