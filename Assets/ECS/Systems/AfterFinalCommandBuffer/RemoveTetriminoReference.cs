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
    [UpdateAfter(typeof(DestroyActiveTetriminosSystem))]
    public partial struct RemoveTetriminoReferencesSystem : ISystem
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

            // Iterate over all the entities that aren't tetriminoes but have a tetrimno owner
            foreach (var (owner, entity)
                in SystemAPI.Query<TetriminoOwner>()
                .WithNone<TetriminoTag>()
                .WithEntityAccess())
            {
                // Remove the references
                ecb.RemoveComponent<CurrentTetrimino>(owner.value);
                ecb.RemoveComponent<TetriminoOwner>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}