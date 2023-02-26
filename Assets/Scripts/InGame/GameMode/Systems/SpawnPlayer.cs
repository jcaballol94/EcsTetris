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
            private void Execute([EntityIndexInQuery] int idx, Entity entity, in PlayerData playerData, in SceneTag scene)
            {
                // Create the player
                var player = ecb.CreateEntity(archetype);
                ecb.SetName(player, "Player");
                ecb.SetComponent(player, new RandomProvider { value = new Unity.Mathematics.Random(seed + (uint)idx * 53) });
                ecb.SetComponent(player, new GridRef { value = playerData.mainGrid });
                ecb.SetComponent(player, new NextGrid { entity = playerData.nextGrid });
                ecb.SetComponent(player, new HoldGrid { value = playerData.holdGrid });
                ecb.SetSharedComponent(player, scene);

                // Mark this as done
                ecb.AddComponent<GameModePlayingTag>(entity);

                // Add a reference to the player in the grid
                ecb.AddComponent(playerData.mainGrid, new PlayerRef { value = player });
            }
        }

        public void OnCreate(ref SystemState state)
        {
            m_playerArchetype = state.EntityManager.CreateArchetype(
                typeof(PlayerTag), // The tag to identify it
                typeof(GridRef), // A reference to this player's grid
                typeof(NextGrid), // The grid where we will show the next tetrimino preview
                typeof(HoldGrid), // The grid where we will show the tetrimino that is on hold
                typeof(InputValues), // The input
                typeof(RandomProvider), typeof(TetriminoQueue), // A provider for the random tetriminos
                typeof(SceneTag) // The scene so it unloads properly
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