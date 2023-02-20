using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial struct SpawnTetriminoSystem : ISystem
    {
        private EntityArchetype m_tetriminoArchetype;

        public void OnCreate(ref SystemState state)
        {
            m_tetriminoArchetype = state.EntityManager.CreateArchetype(
                typeof(LocalGridTransform), typeof(WorldGridTransform), typeof(GridOrientationMatrix), // Transform
                typeof(GridChildren), // Hierarchy
                typeof(DropState), // Movement
                typeof(GridRef), typeof(TetriminoData), typeof(PlayerRef)); // Required data references

            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<AvailableTetriminos>();
            state.RequireForUpdate<GameSkin>();
            state.RequireForUpdate(SystemAPI.QueryBuilder()
                .WithAll<PlayerTag, GridRef>()
                .WithNone<AlreadySpawned>()
                .Build());

            state.RequireForUpdate<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            // Try get the singletons first
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<AvailableTetriminos> availableTetriminos)) return;
            if (!SystemAPI.TryGetSingleton(out GameSkin gameSkin)) return;

            // Get the ecb we'll use
            if (!SystemAPI.TryGetSingleton(out EndVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (gridRef, entity) in SystemAPI
                .Query<RefRO<GridRef>>()
                .WithAll<PlayerTag>()
                .WithNone<AlreadySpawned>()
                .WithEntityAccess())
            {
                // Find a tetrimino type to use
                var tetriminoIdx = UnityEngine.Random.Range(0, availableTetriminos.Length);
                ref var tetriminoData = ref availableTetriminos[tetriminoIdx].asset.Value;

                // Create and initialize the tetrimino
                var tetrimino = ecb.CreateEntity(m_tetriminoArchetype);
                ecb.SetName(tetrimino, "Tetrimino");
                ecb.SetComponent(tetrimino, new LocalGridTransform { position = gameData.spawnPosition, orientation = 0 });
                ecb.SetComponent(tetrimino, new TetriminoData { asset = availableTetriminos[tetriminoIdx].asset });
                ecb.SetComponent(tetrimino, gridRef.ValueRO);
                ecb.SetComponent(tetrimino, new PlayerRef { value = entity });
                ecb.SetComponent(tetrimino, DropState.DefaultDropState);

                // Create and initialize the blocks
                for (int i = 0; i < tetriminoData.blocks.Length; ++i)
                {
                    var block = ecb.Instantiate(gameSkin.blockPrefab);
                    ecb.AppendToBuffer(tetrimino, new GridChildren { value = block });

                    ecb.SetName(block, "Block");
                    ecb.AddComponent(block, new LocalGridTransform { position = tetriminoData.blocks[i] });
                    ecb.AddComponent(block, new GridParent { value = tetrimino });
                    ecb.SetComponent(block, new Unity.Rendering.URPMaterialPropertyBaseColor { Value = tetriminoData.color });
                    ecb.AddComponent(block, gridRef.ValueRO);
                }

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}