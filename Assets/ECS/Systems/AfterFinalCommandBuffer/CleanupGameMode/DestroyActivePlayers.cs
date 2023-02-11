using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AfterFinalCommandBufferSystemGroup))]
    public partial struct DestroyActivePlayersSystem : ISystem
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

            foreach (var (players, gameModeEntity) in 
                SystemAPI.Query<DynamicBuffer<ActivePlayerBuffer>>()
                .WithNone<PlayerDefinitionBuffer>().WithEntityAccess())
            {
                foreach (var player in players)
                {
                    ecb.DestroyEntity(player.value);
                }

                ecb.RemoveComponent<ActivePlayerBuffer>(gameModeEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}