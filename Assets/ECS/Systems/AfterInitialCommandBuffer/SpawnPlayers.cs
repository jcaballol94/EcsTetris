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
    public partial struct SpawnPlayersSystem : ISystem
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
            foreach (var (playerDefinitions, tetriminos, blockPfefab, spawnPosition, gameModeEntity) in 
                SystemAPI.Query<DynamicBuffer<PlayerDefinitionBuffer>, DynamicBuffer<AvailableTetriminoBuffer>, RefRO<BlockPrefab>, GameSettings>()
                .WithNone<ActivePlayerBuffer>()
                .WithEntityAccess())
            {
                var playerBuffer = ecb.AddBuffer<ActivePlayerBuffer>(gameModeEntity);

                foreach (var playerDef in playerDefinitions)
                {
                    var playerEntity = ecb.CreateEntity();
                    ecb.SetName(playerEntity, "Player");

                    // Create a player and give it a reference to the game mode so it can be initialized
                    ecb.AddComponent<PlayerTag>(playerEntity);
                    ecb.AddComponent(playerEntity, blockPfefab.ValueRO);
                    ecb.AddSharedComponent(playerEntity, spawnPosition);
                    ecb.AddComponent(playerEntity, new GridRef { value = playerDef.grid });
                    ecb.AddComponent<UnmanagedInput>(playerEntity);

                    var tetriminoesBuffer = ecb.AddBuffer<AvailableTetriminoBuffer>(playerEntity);
                    foreach (var tetrimino in tetriminos)
                    {
                        tetriminoesBuffer.Add(tetrimino);
                    }

                    playerBuffer.Add(new ActivePlayerBuffer { value = playerEntity });
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}