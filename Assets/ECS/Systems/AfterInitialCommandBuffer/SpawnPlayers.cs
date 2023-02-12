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
            foreach (var (spawner, gameModeEntity) in 
                SystemAPI.Query<PlayerSpawner>()
                .WithNone<ActivePlayerBuffer>()
                .WithEntityAccess())
            {
                var playerBuffer = ecb.AddBuffer<ActivePlayerBuffer>(gameModeEntity);

                for (int i = 0; i < spawner.PlayersToCreate; ++i)
                {
                    playerBuffer.Add(new ActivePlayerBuffer { value = spawner.SpawnEntity(ref ecb, i) });
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}