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
            foreach (var (playerDefinitions, gameModeEntity) in 
                SystemAPI.Query<DynamicBuffer<PlayerDefinitionBuffer>>()
                .WithNone<ActivePlayerBuffer>()
                .WithEntityAccess())
            {
                if (playerDefinitions.Length == 0)
                    continue;

                // Store the references to the players for the cleanup
                var playerBuffer = ecb.AddBuffer<ActivePlayerBuffer>(gameModeEntity);

                int i = 0;
                foreach (var playerDef in playerDefinitions)
                {
                    var playerEntity = ecb.CreateEntity();
                    ecb.SetName(playerEntity, "Player_" + i++);
                    PlayerAspect.SetupPlayer(playerEntity, ecb);
                    playerBuffer.Add(new ActivePlayerBuffer { value = playerEntity });
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}