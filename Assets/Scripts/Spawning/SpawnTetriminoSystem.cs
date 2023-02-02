using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

namespace Tetris
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class SpawnSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(SpawnSystemGroup))]
    public partial struct SpawnTetriminoSystem : ISystem
    {
        private EntityQuery m_blocksQuery;
        private EntityQuery m_gameModeQuery;

        public void OnCreate(ref SystemState state)
        {
            m_blocksQuery = SystemAPI.QueryBuilder().WithAll<TetriminoTag>().Build();
            m_gameModeQuery = SystemAPI.QueryBuilder().WithAll<TetrisGameMode, AvailableTetrimino>().Build();

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

            // Try to get the available tetriminos
            if (!m_gameModeQuery.TryGetSingletonBuffer<AvailableTetrimino>(out var availableTetriminos, true))
                return;

            // Get a random tetrimino type
            var tetriminoIdx = UnityEngine.Random.Range(0, availableTetriminos.Length);

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Create the terimino
            var newTetrimino = state.EntityManager.CreateEntity();
            ecb.AddComponent<TetriminoTag>(newTetrimino);
            // Link the type
            ecb.AddComponent(newTetrimino, new TetriminoDefinitionRef { value = availableTetriminos[tetriminoIdx].value });
            // Link a reference to the grid
            ecb.AddComponent(newTetrimino, new GridReference { value = gameMode.mainGrid });
            // Put it in the right position
            ecb.AddComponent(newTetrimino, new Position { value = gameMode.spawnPosition });

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(SpawnTetriminoSystem))]
    public partial struct SpawnBlocksSystem : ISystem
    {
        private EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<TetriminoDefinitionRef, GridReference>().WithNone<ChildRef>().Build();

            state.RequireForUpdate(m_query);
            state.RequireForUpdate<TetrisGameMode>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            // Try to get the data required
            if (!SystemAPI.TryGetSingleton<TetrisGameMode>(out var gameMode))
                return;

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

            var blockLookup = SystemAPI.GetBufferLookup<TetriminoBlockDefinition>(true);
            var colorLookup = SystemAPI.GetComponentLookup<TetriminoColorDefinition>(true);

            new SpawnBlocksJob
            {
                blockLookup = blockLookup,
                colorLookup = colorLookup,
                ecb = ecb,
                gameMode = gameMode
            }.Run(m_query);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct SpawnBlocksJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<TetriminoColorDefinition> colorLookup;
        [ReadOnly] public BufferLookup<TetriminoBlockDefinition> blockLookup;
        public EntityCommandBuffer ecb;
        public TetrisGameMode gameMode;

        [BurstCompile]
        public void Execute(Entity tetriminoEntity, in TetriminoDefinitionRef tetriminoDefinition, in GridReference gridRef)
        {
            // Add the list of children
            ecb.AddBuffer<ChildRef>(tetriminoEntity);

            // Get the color of the blocks
            var color = colorLookup[tetriminoDefinition.value];

            // Iterate over all the required blocks
            var blocksDefinition = blockLookup[tetriminoDefinition.value];
            for (int i = 0; i < blocksDefinition.Length; i++)
            {
                // Create the block and link it with the tetrimino
                var block = ecb.Instantiate(gameMode.blockPrefab);
                ecb.AppendToBuffer(tetriminoEntity, new ChildRef { value = block });
                ecb.AddComponent(block, new TetriminoRef { value = tetriminoEntity });

                // Set the position based on the definition
                ecb.AddComponent(block, new LocalPosition { value = blocksDefinition[i].value });
                // Set a reference to the grid to be able to compute its position
                ecb.AddComponent(block, gridRef);
                // Set the color
                ecb.AddComponent(block, new URPMaterialPropertyBaseColor { Value = color.value });
            }
        }
    }
}