using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateBefore(typeof(SpawnTetriminoSystem))]
    public partial struct PlaceTetriminoSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlaceTetriminoEvent>();
            state.RequireForUpdate<RefreshGridCollisionsEvent>();
            state.RequireForUpdate<RequestSpawnTetriminoEvent>();
            state.RequireForUpdate<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlaceTetriminoEvent> events, true)) return;
            if (events.Length == 0) return;

            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<RequestSpawnTetriminoEvent> spawnEvents)) return;
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<RefreshGridCollisionsEvent> gridEvents)) return;

            if (!SystemAPI.TryGetSingleton(out EndVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var ev in events)
            {
                var children = state.EntityManager.GetBuffer<GridChildren>(ev.tetrimino, true);

                // Unparent all the children
                foreach (var child in children)
                {
                    ecb.RemoveComponent<GridParent>(child.value);
                    // Set it as static so we can collide with it
                    ecb.AddComponent<StaticBlockTag>(child.value);
                }

                // Finally, destroy the tetrimino
                ecb.DestroyEntity(ev.tetrimino);

                // Request a new tetrimino to be spawned
                var playerRef = state.EntityManager.GetComponentData<PlayerRef>(ev.tetrimino);
                spawnEvents.Add(new RequestSpawnTetriminoEvent { player = playerRef.value });
                // Request that the grid is refreshed
                var gridRef = state.EntityManager.GetSharedComponent<GridRef>(ev.tetrimino);
                gridEvents.Add(new RefreshGridCollisionsEvent { grid = gridRef.value });
            }
        }
    }
}