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
    [UpdateAfter(typeof(UpdateGridCollisionsSystem))]
    public partial struct DetectLinesSystem : ISystem
    {
        [WithNone(typeof(DetectedLinesBuffer))]
        public partial struct DetectLinesJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            private void Execute(Entity entity, in GridCollisions collisions)
            {
                var lines = new DynamicBuffer<DetectedLinesBuffer>();

                var gridSize = collisions.GridSize;
                for (int i = 0; i < gridSize.y; ++i)
                {
                    bool line = true;
                    for (int j = 0; line && j < gridSize.x; ++j)
                    {
                        line = !collisions.IsPositionAvailable(new int2(j, i));
                    }

                    if (line)
                    {
                        if (!lines.IsCreated)
                            lines = ecb.AddBuffer<DetectedLinesBuffer>(entity);

                        lines.Add(new DetectedLinesBuffer { lineY = i });
                    }
                }
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            new DetectLinesJob
            {
                ecb = ecb
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}