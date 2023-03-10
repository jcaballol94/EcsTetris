using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GenerateRandomTetriminosSystem))]
    [UpdateAfter(typeof(InitializeGridCellsSystem))]
    public partial struct SpawnTetriminoSystem : ISystem
    {
        private EntityArchetype m_tetriminoArchetype;
        private GridCollisions.Lookup m_colliderLookup;

        [WithAll(typeof(PlayerTag))]
        [WithNone(typeof(TetriminoRef), typeof(GameOverTag), typeof(SpawnTetriminoDelay))]
        public partial struct SpawnTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public EntityArchetype archetype;

            public GameData gameData;
            [ReadOnly] public GridCollisions.Lookup colliderLookup;

            private void Execute(Entity entity, in CurrentScore score, in NextTetrimino next, in GridRef gridRef, in SceneTag scene)
            {
                // Remove the next so we trigger generating a new one
                ecb.RemoveComponent<NextTetrimino>(entity);

                // Check if the tetrimino can be spawned
                var collider = colliderLookup[gridRef.value];
                bool canSpawn = true;
                for (int i = 0; canSpawn && i < next.data.blocks.Length; i++)
                {
                    canSpawn = collider.IsPositionValid(gameData.spawnPosition + next.data.blocks[i]);
                }

                // Create and initialize the tetrimino
                if (canSpawn)
                {
                    var tetrimino = ecb.CreateEntity(archetype);
                    ecb.SetName(tetrimino, "Tetrimino");
                    ecb.SetComponent(tetrimino, new TetriminoPosition { position = gameData.spawnPosition, orientation = 0 });
                    ecb.SetComponent(tetrimino, next.data);
                    ecb.SetComponent(tetrimino, gridRef);
                    ecb.SetComponent(tetrimino, new PlayerCleanupRef { value = entity });
                    ecb.SetComponent(tetrimino, DropState.DefaultDropState);
                    ecb.SetComponent(tetrimino, new DropSpeed { value = gameData.GetSpeedForLevel(score.level) });
                    ecb.SetSharedComponent(tetrimino, scene);

                    // Save a reference to the tetrimino
                    ecb.AddComponent(entity, new TetriminoRef { value = tetrimino });
                }
                else
                {
                    // We couldn't spawn, so it's a game over!
                    ecb.AddComponent<GameOverTag>(entity);
                }
            }
        }

        public void OnCreate(ref SystemState state)
        {
            m_tetriminoArchetype = state.EntityManager.CreateArchetype(
                typeof(TetriminoTag),
                typeof(TetriminoPosition), // Transform
                typeof(DropState), typeof(DropSpeed), // Movement
                typeof(GridRef), typeof(TetriminoData), typeof(PlayerCleanupRef), // Required data references
                typeof(SceneTag) // Link it to the scene so it can be properly unloaded
                );

            m_colliderLookup = new GridCollisions.Lookup(ref state, true);

            // All this data needs to be available
            state.RequireForUpdate<GameData>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Try get the singletons first
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            m_colliderLookup.Update(ref state);

            state.Dependency.Complete();
            new SpawnTetriminoJob
            {
                archetype = m_tetriminoArchetype,
                ecb = ecb,
                gameData = gameData,
                colliderLookup = m_colliderLookup
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}