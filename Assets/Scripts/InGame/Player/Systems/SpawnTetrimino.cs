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
    [UpdateAfter(typeof(SpawnPlayerSystem))]
    public partial struct SpawnTetriminoSystem : ISystem
    {
        private EntityArchetype m_tetriminoArchetype;

        [BurstCompile]
        [WithAll(typeof(PlayerTag))]
        [WithNone(typeof(TetriminoRef))]
        public partial struct SpawnTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public EntityArchetype archetype;

            public GameData gameData;
            [ReadOnly] public DynamicBuffer<AvailableTetriminos> availableTetriminos;

            [BurstCompile]
            private void Execute(Entity entity, ref RandomProvider random, in GridRef gridRef)
            {
                // Find a tetrimino type to use
                var tetriminoIdx = (int)random.value.NextUInt() % availableTetriminos.Length;
                ref var tetriminoData = ref availableTetriminos[tetriminoIdx].asset.Value;

                // Create and initialize the tetrimino
                var tetrimino = ecb.CreateEntity(archetype);
                ecb.SetName(tetrimino, "Tetrimino");
                ecb.SetComponent(tetrimino, new TetriminoPosition { position = gameData.spawnPosition, orientation = 0 });
                ecb.SetComponent(tetrimino, new TetriminoData { asset = availableTetriminos[tetriminoIdx].asset });
                ecb.SetSharedComponent(tetrimino, gridRef);
                ecb.SetComponent(tetrimino, new PlayerCleanupRef { value = entity });
                ecb.SetComponent(tetrimino, DropState.DefaultDropState);

                // Save a reference to the tetrimino
                ecb.AddComponent(entity, new TetriminoRef { value = tetrimino });
            }
        }

        public void OnCreate(ref SystemState state)
        {
            m_tetriminoArchetype = state.EntityManager.CreateArchetype(
                typeof(TetriminoPosition), // Transform
                typeof(DropState), // Movement
                typeof(GridRef), typeof(TetriminoData), typeof(PlayerCleanupRef)); // Required data references

            // All this data needs to be available
            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<AvailableTetriminos>();
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
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<AvailableTetriminos> availableTetriminos, true)) return;

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            new SpawnTetriminoJob
            {
                archetype = m_tetriminoArchetype,
                ecb = ecb,
                availableTetriminos = availableTetriminos,
                gameData = gameData
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}