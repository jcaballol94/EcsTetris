using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(EventsSystemGroup))]
    public partial struct PlaceTetriminoSystem : ISystem
    {
        [BurstCompile]
        public partial struct PlaceTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public BufferLookup<GridChildren> childrenLookup;

            [BurstCompile]
            private void Execute([ChunkIndexInQuery] int id, in PlaceTetriminoEvent ev)
            {
                var children = childrenLookup[ev.tetrimino];

                // Unparent all the children
                foreach (var child in children)
                {
                    ecb.RemoveComponent<GridParent>(id, child.value);
                }

                // Finally, destroy the tetrimino
                ecb.DestroyEntity(id, ev.tetrimino);
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out BeginVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var childrenLookup = SystemAPI.GetBufferLookup<GridChildren>(true);

            new PlaceTetriminoJob
            {
                childrenLookup = childrenLookup,
                ecb = ecb
            }.ScheduleParallel();
        }
    }
}