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
    [UpdateAfter(typeof(DestroyActivePlayersSystem))]
    public partial struct DestroyActiveTetriminosSystem : ISystem
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

            // Iterate over entities with a tetrimino attached but no player tag
            foreach (var (tetriminoRef, playerEntity)
                in SystemAPI.Query<CurrentTetrimino>()
                .WithNone<PlayerTag>()
                .WithEntityAccess())
            {
                // Unregister myself
                ecb.RemoveComponent<TetriminoOwner>(tetriminoRef.value);
                // Destroy the tetrimino
                ecb.DestroyEntity(tetriminoRef.value);

                // Remove the reference
                ecb.RemoveComponent<CurrentTetrimino>(playerEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}