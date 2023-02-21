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
    public partial struct SpawnPlayerSystem : ISystem
    {
        private EntityArchetype m_playerArchetype;

        public void OnCreate(ref SystemState state)
        {
            m_playerArchetype = state.EntityManager.CreateArchetype(
                typeof(PlayerTag), // The tag to identify it
                typeof(GridRef), // A reference to this player's grid
                typeof(InputValues) // The input
                );

            state.RequireForUpdate<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<RequestSpawnTetriminoEvent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<RequestSpawnTetriminoEvent>(out var eventBufferEntity)) return;
            if (!SystemAPI.TryGetSingleton(out BeginVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSingleton)) return;
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (playerData, entity) in SystemAPI
                .Query<RefRO<PlayerData>>()
                .WithNone<AlreadySpawned>()
                .WithEntityAccess())
            {
                // Create the player
                var player = ecb.CreateEntity(m_playerArchetype);
                ecb.SetName(player, "Player");
                ecb.SetComponent(player, new GridRef { value = playerData.ValueRO.grid });

                // Request the first tetrimino to start playing, use the command buffer so the event isn't created until the player also exists
                ecb.AppendToBuffer(eventBufferEntity, new RequestSpawnTetriminoEvent { player = player });

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}