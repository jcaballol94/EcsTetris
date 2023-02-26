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
    [UpdateBefore(typeof(RemoveLinesSystem))]
    public partial struct InitializeFadeOutSystem : ISystem
    {
        [BurstCompile]
        [WithAll(typeof(TrackedByGridTag))]
        [WithNone(typeof(FadeOut))]
        public partial struct StartAnimationJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public BufferLookup<DetectedLinesBuffer> linesLookup;

            [BurstCompile]
            private void Execute(Entity entity, in BlockPosition position, in GridRef gridRef)
            {
                // Check that there are lines
                if (!linesLookup.HasBuffer(gridRef.value))
                    return;
                var lines = linesLookup[gridRef.value];

                var pos = position.position;
                foreach (var line in lines)
                {
                    // If we are in a line to remove, delete
                    if (line.lineY == pos.y)
                    {
                        ecb.AddComponent<WaitForAnimation>(gridRef.value);
                        ecb.AddComponent(entity, FadeOut.Start);
                        return;
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
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            var linesLookup = SystemAPI.GetBufferLookup<DetectedLinesBuffer>(true);

            new StartAnimationJob
            {
                ecb = ecb,
                linesLookup = linesLookup
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}