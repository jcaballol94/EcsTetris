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
                typeof(LocalGridPosition), typeof(WorldGridPosition), // Positioning
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

            foreach (var (gameData, tetriminoData, gridRef, entity) in SystemAPI
                .Query<RefRO<GameData>, RefRO<TetriminoData>, GridRef>()
                .WithNone<AlreadySpawned>()
                .WithEntityAccess())
            {
                var tetrimino = ecb.CreateEntity(m_tetriminoArchetype);
                ecb.SetName(tetrimino, "Tetrimino");
                ecb.SetComponent(tetrimino, new LocalGridPosition { value = gameData.ValueRO.spawnPosition });
                ecb.SetComponent(tetrimino, tetriminoData.ValueRO);
                ecb.SetSharedComponent(tetrimino, gridRef);

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}