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

        [BurstCompile]
        [WithNone(typeof(GameModePlayingTag))]
        public partial struct SpawnPlayerJob : IJobEntity
        {
            public EntityArchetype archetype;
            public EntityCommandBuffer ecb;

            public uint seed;

            [BurstCompile]
            private void Execute([EntityIndexInQuery] int idx, Entity entity, in PlayerData playerData)
            {
                Debug.Log("Spawn Player Job");

                // Create the player
                var player = ecb.CreateEntity(archetype);
                ecb.SetName(player, "Player");
                ecb.SetComponent(player, new RandomProvider { value = new Unity.Mathematics.Random(seed + (uint)idx * 53) });
                ecb.SetSharedComponent(player, new GridRef { value = playerData.grid });

                // Mark this as done
                ecb.AddComponent<GameModePlayingTag>(entity);
            }
        }

        public void OnCreate(ref SystemState state)
        {
            m_playerArchetype = state.EntityManager.CreateArchetype(
                typeof(PlayerTag), // The tag to identify it
                typeof(GridRef), // A reference to this player's grid
                typeof(InputValues), // The input
                typeof(RandomProvider) // A provider for the random tetriminos
                );
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

            var seed = (uint)System.DateTime.UtcNow.Ticks;
            new SpawnPlayerJob
            {
                archetype = m_playerArchetype,
                ecb = ecb,
                seed = seed
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}