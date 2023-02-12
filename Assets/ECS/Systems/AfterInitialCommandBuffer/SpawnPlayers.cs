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
            foreach (var (gameDataRef, blockPrefab, gameModeEntity) in 
                SystemAPI.Query<RefRO<GameModeData>, RefRO<BlockPrefab>>()
                .WithAll<ActiveGameModeTag>()
                .WithNone<ActivePlayerBuffer>()
                .WithEntityAccess())
            {
                ref var gameData = ref gameDataRef.ValueRO.value.Value;
                if (gameData.players.Length == 0)
                    continue;

                // Store the references to the players for the cleanup
                var playerBuffer = ecb.AddBuffer<ActivePlayerBuffer>(gameModeEntity);

                for (int i = 0; i < gameData.players.Length; i++)
                {
                    var playerDef = gameData.players[i];
                    var playerEntity = ecb.CreateEntity();
                    ecb.SetName(playerEntity, "Player_" + i++);

                    ecb.AddComponent<PlayerTag>(playerEntity);
                    ecb.AddComponent(playerEntity, gameDataRef.ValueRO);
                    ecb.AddComponent(playerEntity, blockPrefab.ValueRO);

                    playerBuffer.Add(new ActivePlayerBuffer { value = playerEntity });
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}