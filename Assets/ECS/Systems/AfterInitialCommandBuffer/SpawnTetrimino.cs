using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AfterInitialCommandBufferSystemGroup))]
    [UpdateAfter(typeof(SpawnPlayersSystem))]
    public partial struct SpawnTetriminoSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Get all the players without tetriminoes
            foreach (var (availableTetriminoes, blockPrefab, spawnPosition, grid, playerEntity) in 
                SystemAPI.Query<DynamicBuffer<AvailableTetriminoBuffer>, RefRO<BlockPrefab>, GameSettings, RefRO<GridRef>>()
                .WithAll<PlayerTag>()
                .WithNone<CurrentTetrimino>()
                .WithEntityAccess())
            {
                // Get a random tetrimino type
                var type = UnityEngine.Random.Range(0, availableTetriminoes.Length);

                // Create and setup the entity
                var tetriminoEntity = ecb.CreateEntity();
                ecb.SetName(tetriminoEntity, "Tetrimino");
                ecb.AddComponent<TetriminoTag>(tetriminoEntity);

                ecb.AddComponent<HasCollisionsTag>(tetriminoEntity);
                ecb.AddComponent(tetriminoEntity, blockPrefab.ValueRO);
                ecb.AddComponent(tetriminoEntity, new TetriminoType { asset = availableTetriminoes[type].asset });
                ecb.AddComponent(tetriminoEntity, new TetriminoOwner { value = playerEntity });
                ecb.AddComponent(tetriminoEntity, new InputProvider { value = playerEntity });
                ecb.AddComponent(tetriminoEntity, new Position { value = spawnPosition.spawnPosition });
                ecb.AddComponent(tetriminoEntity, grid.ValueRO);

                // Store a ref in the player entity
                ecb.AddComponent(playerEntity, new CurrentTetrimino { value = tetriminoEntity });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}