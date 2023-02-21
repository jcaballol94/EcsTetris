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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateBefore(typeof(SpawnTetriminoSystem))]
    public partial struct PlaceTetriminoSystem : ISystem
    {
        [BurstCompile]
        public partial struct PlaceTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public BufferLookup<GridChildren> childrenLookup;
            public DynamicBuffer<RequestSpawnTetriminoEvent> spawnEvents;

            [BurstCompile]
            private void Execute(in PlaceTetriminoEvent ev)
            {
                var children = childrenLookup[ev.tetrimino];

                // Unparent all the children
                foreach (var child in children)
                {
                    ecb.RemoveComponent<GridParent>(child.value);
                }

                // Finally, destroy the tetrimino
                ecb.DestroyEntity(ev.tetrimino);
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlaceTetriminoEvent>();
            state.RequireForUpdate<RequestSpawnTetriminoEvent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlaceTetriminoEvent> events, true)) return;
            if (events.Length == 0) return;

            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<RequestSpawnTetriminoEvent> spawnEvents)) return;

            if (!SystemAPI.TryGetSingleton(out EndSimulationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var ev in events)
            {
                var children = state.EntityManager.GetBuffer<GridChildren>(ev.tetrimino, true);

                // Unparent all the children
                foreach (var child in children)
                {
                    ecb.RemoveComponent<GridParent>(child.value);
                }

                // Finally, destroy the tetrimino
                ecb.DestroyEntity(ev.tetrimino);

                // Request a new tetrimino to be spawned
                var playerRef = state.EntityManager.GetComponentData<PlayerRef>(ev.tetrimino);
                spawnEvents.Add(new RequestSpawnTetriminoEvent { player = playerRef.value });
            }
        }
    }
}