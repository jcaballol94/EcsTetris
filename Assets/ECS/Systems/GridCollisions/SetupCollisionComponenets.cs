using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GridCollisionsSystemGroup), OrderFirst = true)]
    public partial struct SetupCollisionComponenetsSystem : ISystem
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

            var sizeLookup = SystemAPI.GetComponentLookup<GridSize>(true);
            foreach (var (grid, entity)
                in SystemAPI.Query<GridRef>()
                .WithAll<HasCollisionsTag>()
                .WithNone<GridSize>()
                .WithEntityAccess())
            {
                ecb.AddComponent(entity, sizeLookup[grid.value]);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}