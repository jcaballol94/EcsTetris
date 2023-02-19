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

        public void OnCreate(ref SystemState state)
        {
            m_playerArchetype = state.EntityManager.CreateArchetype(
                typeof(PlayerTag), // The tag to identify it
                typeof(GridRef), // A reference to this player's grid
                typeof(InputValues) // The input
                );

        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = state.EntityManager.World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer();

            foreach (var (playerData, entity) in SystemAPI
                .Query<RefRO<PlayerData>>()
                .WithNone<AlreadySpawned>()
                .WithEntityAccess())
            {
                var player = ecb.CreateEntity(m_playerArchetype);
                ecb.SetName(player, "Player");
                ecb.SetComponent(player, new GridRef { value = playerData.ValueRO.grid });

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}