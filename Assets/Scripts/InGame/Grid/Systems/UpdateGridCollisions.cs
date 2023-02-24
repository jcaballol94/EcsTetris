using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InitializeGridCellsSystem))]
    public partial struct UpdateGridCollisionsSystem : ISystem
    {
        [WithAll(typeof(StaticBlockTag))]
        [WithNone(typeof(TrackedByGridTag))]
        public partial struct UpdateGridCollisionsJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            public GridCollisions.Lookup collisionsLookup;

            private void Execute(Entity entity, in BlockPosition transform, in GridRef gridRef)
            {
                var collider = collisionsLookup[gridRef.value];
                collider.TakePosition(transform.position);

                ecb.AddComponent<TrackedByGridTag>(entity);
            }
        }

        private GridCollisions.Lookup m_collisionsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();

            m_collisionsLookup = new GridCollisions.Lookup(ref state, false);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out EndInitializationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            m_collisionsLookup.Update(ref state);

            new UpdateGridCollisionsJob
            {
                collisionsLookup = m_collisionsLookup,
                ecb = ecb
            }.Run();
        }
    }
}