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
            foreach (var (gameDataRef, playerEntity) in 
                SystemAPI.Query<RefRO<GameModeData>>()
                .WithAll<PlayerTag>()
                .WithNone<CurrentTetrimino>()
                .WithEntityAccess())
            {
                ref var gameData = ref gameDataRef.ValueRO.value.Value;

                // Get a random tetrimino type
                var type = UnityEngine.Random.Range(0, gameData.tetriminos.Length);

                // Create and setup the entity
                var tetriminoEntity = ecb.CreateEntity();
                ecb.SetName(tetriminoEntity, "tetrimino");

                ecb.AddComponent<TetriminoTag>(tetriminoEntity);
                ecb.AddComponent(tetriminoEntity, gameDataRef.ValueRO);
                ecb.AddComponent(tetriminoEntity, new TetriminoType { idx = type });
                ecb.AddComponent(tetriminoEntity, new TetriminoOwner { value = playerEntity });

                // Store a ref in the player entity
                ecb.AddComponent(playerEntity, new CurrentTetrimino { value = tetriminoEntity });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}