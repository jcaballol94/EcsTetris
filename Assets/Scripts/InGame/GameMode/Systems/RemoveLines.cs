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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(DetectLinesSystem))]
    public partial struct RemoveLinesSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(WaitForAnimation))]
        public partial struct RemoveLinesCollisionsJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(ref DynamicBuffer<GridCellData> cells, in DynamicBuffer<DetectedLinesBuffer> events, in GridBounds bounds)
            {
                if (events.Length == 0)
                    return;

                for (int i = 0; i < bounds.size.y; ++i)
                {
                    // Calculate how much to move
                    var offset = 0;
                    foreach (var ev in events)
                    {
                        if (ev.lineY < i)
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
        [WithAll(typeof(TrackedByGridTag))]
        public partial struct RemoveLinesEntitiesJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public BufferLookup<DetectedLinesBuffer> linesLookup;
            [ReadOnly] public ComponentLookup<WaitForAnimation> animatinLookup;

            [BurstCompile]
            private void Execute(Entity entity, ref BlockPosition position, in GridRef gridRef)
            {
                // Wait if required
                if (animatinLookup.HasComponent(gridRef.value))
                    return;
                // Check that there are lines to remove
                if (!linesLookup.HasBuffer(gridRef.value))
                    return;
                var lines = linesLookup[gridRef.value];

                var pos = position.position;
                // Calculate how much to move
                var offset = 0;
                foreach (var line in lines)
                {
                    // If we are in a line to remove, delete
                    if (line.lineY == pos.y)
                    {
                        //Debug.Log("Remove entity at " + pos);
                        ecb.DestroyEntity(entity);
                        return;
                    }
                    else if (line.lineY < pos.y)
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
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();

            state.RequireForUpdate<DetectedLinesBuffer>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>(out var ecbSystem)) return;
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            // Remove the entities in the line and move the ones above
            var eventLookup = SystemAPI.GetBufferLookup<DetectedLinesBuffer>(true);
            var waitLookup = SystemAPI.GetComponentLookup<WaitForAnimation>(true);
            state.Dependency = new RemoveLinesEntitiesJob
            {
                ecb = ecb,
                linesLookup = eventLookup,
                animatinLookup = waitLookup
            }.Schedule(state.Dependency);

            // Update the collisions
            state.Dependency = new RemoveLinesCollisionsJob().ScheduleParallel(state.Dependency);

            // The lines have been processed, we can remove them
            ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            ecb.RemoveComponent<DetectedLinesBuffer>(SystemAPI.QueryBuilder()
                .WithAll<DetectedLinesBuffer>()
                .WithNone<WaitForAnimation>()
                .Build());
        }
    }
}