using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct TestSpawnerSystem : ISystem
    {
        private EntityArchetype m_tetriminoArchetype;

        public void OnCreate(ref SystemState state)
        {
            m_tetriminoArchetype = state.EntityManager.CreateArchetype(
                typeof(LocalGridTransform), typeof(WorldGridTransform), typeof(GridOrientationMatrix), // Transform
                typeof(GridChildren), // Hierarchy
                typeof(GridRef), typeof(TetriminoData)); // Required data references
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = state.World.GetExistingSystemManaged<BeginVariableRateSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer();

            Debug.Log("Spawn");

            foreach (var (gameData, tetriminoData, playerData, skin, entity) in SystemAPI
                .Query<RefRO<GameData>, RefRO<TetriminoData>, RefRO<PlayerData>, RefRO<GameSkin>>()
                .WithNone<AlreadySpawned>()
                .WithEntityAccess())
            {
                // Create and initialize the tetrimino
                var tetrimino = ecb.CreateEntity(m_tetriminoArchetype);
                ecb.SetName(tetrimino, "Tetrimino");
                ecb.SetComponent(tetrimino, new LocalGridTransform { position = gameData.ValueRO.spawnPosition, orientation = 0 });
                ecb.SetComponent(tetrimino, tetriminoData.ValueRO);
                ecb.SetSharedComponent(tetrimino, new GridRef { value = playerData.ValueRO.grid });

                // Create and initialize the blocks
                for (int i = 0; i < tetriminoData.ValueRO.blocks.Length; ++i)
                {
                    var block = ecb.Instantiate(skin.ValueRO.blockPrefab);
                    ecb.AppendToBuffer(tetrimino, new GridChildren { value = block });

                    ecb.SetName(block, "Block");
                    ecb.AddComponent(block, new LocalGridTransform { position = tetriminoData.ValueRO.blocks[i] });
                    ecb.AddComponent(block, new GridParent { value = tetrimino });
                    ecb.AddSharedComponent(block, new GridRef { value = playerData.ValueRO.grid });
                }

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}