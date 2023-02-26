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
    [UpdateAfter(typeof(DetectLinesSystem))]
    [UpdateBefore(typeof(RemoveLinesSystem))]
    public partial struct AddLinesToScoreSystem : ISystem
    {
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
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Remove the tag to process the next set of lines
            state.EntityManager.RemoveComponent<ScoredLinesAdded>(SystemAPI.QueryBuilder()
                .WithAll<ScoredLinesAdded>()
                .WithNone<DetectedLinesBuffer>()
                .Build());

            var scoreLookup = SystemAPI.GetComponentLookup<CurrentScore>();

            foreach (var (lines, player) in SystemAPI
                .Query<DynamicBuffer<DetectedLinesBuffer>, RefRO<PlayerRef>>()
                .WithNone<ScoredLinesAdded>())
            {
                var scores = scoreLookup.GetRefRW(player.ValueRO.value, false);

                var numLines = lines.Length;

                switch(numLines)
                {
                    case 1:
                        numLines = 1;
                        break;
                    case 2:
                        numLines = 3;
                        break;
                    case 3:
                        numLines = 5;
                        break;
                    case 4:
                        numLines = 8;
                        break;
                }

                scores.ValueRW.lines += numLines;
                scores.ValueRW.score += numLines * scores.ValueRO.level * 100;
                scores.ValueRW.level = (scores.ValueRO.lines / 10) + 1;
            }

            state.EntityManager.AddComponent<ScoredLinesAdded>(SystemAPI.QueryBuilder()
                .WithAll<DetectedLinesBuffer>()
                .WithNone<ScoredLinesAdded>()
                .Build());
        }
    }
}