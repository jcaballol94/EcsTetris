using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnPlayerSystem : ISystem
    {
        private EntityArchetype m_playerArchetype;
        private EntityArchetype m_startPlayingEventArchetype;

        public void OnCreate(ref SystemState state)
        {
            m_playerArchetype = state.EntityManager.CreateArchetype(
                typeof(PlayerTag), // The tag to identify it
                typeof(GridRef), // A reference to this player's grid
                typeof(InputValues) // The input
                );

            m_startPlayingEventArchetype = state.EntityManager.CreateArchetype(
                typeof(EventTag), typeof(RequestSpawnTetriminoEvent));

            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton)) return;
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

                // Request the first tetrimino to start playing
                var ev = ecb.CreateEntity(m_startPlayingEventArchetype);
                ecb.SetComponent(ev, new RequestSpawnTetriminoEvent { player = player });

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}