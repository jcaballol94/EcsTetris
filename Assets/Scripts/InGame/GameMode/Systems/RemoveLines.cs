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
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(DetectLinesSystem))]
    public partial struct RemoveLinesSystem : ISystem
    {
        [BurstCompile]
        public partial struct RemoveLinesCollisionsJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(ref DynamicBuffer<GridCellData> cells, in DynamicBuffer<RemoveLineEvent> events, in GridBounds bounds)
            {
                if (events.Length == 0)
                    return;

                for (int i = 0; i < bounds.size.y; ++i)
                {
                    // Calculate how much to move
                    var offset = 0;
                    foreach (var ev in events)
                    {
                        if (ev.lineY <= i)
                            offset++;
                    }
                    if (offset == 0) continue;

                    // Move the values
                    var writeLine = math.max(0, i - offset);
                    for (int j = 0; j < bounds.size.x; ++j)
                    {
                        cells[writeLine * bounds.size.x + j] = cells[i * bounds.size.x + j];
                    }
                }
            }
        }

        [BurstCompile]
        [WithAll(typeof(StaticBlockTag))]
        public partial struct RemoveLinesEntitiesJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public BufferLookup<RemoveLineEvent> eventLookup;

            [BurstCompile]
            private void Execute(Entity entity, ref BlockPosition position, in GridRef gridRef)
            {
                var events = eventLookup[gridRef.value];
                if (events.Length == 0)
                    return;

                var pos = position.position;
                // Calculate how much to move
                var offset = 0;
                foreach (var ev in events)
                {
                    // If we are in a line to remove, delete
                    if (ev.lineY == pos.y)
                    {
                        ecb.DestroyEntity(entity);
                        return;
                    }
                    else if (ev.lineY < pos.y)
                    {
                        offset++;
                    }
                }

                // Move the block
                pos.y -= offset;
                position.position = pos;
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>(out var ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            // Remove the entities in the line and move the ones above
            var eventLookup = SystemAPI.GetBufferLookup<RemoveLineEvent>(true);
            state.Dependency = new RemoveLinesEntitiesJob
            {
                ecb = ecb,
                eventLookup = eventLookup

            }.Schedule(state.Dependency);

            // Update the collisions
            state.Dependency.Complete();
            new RemoveLinesCollisionsJob().Run();
        }
    }
}