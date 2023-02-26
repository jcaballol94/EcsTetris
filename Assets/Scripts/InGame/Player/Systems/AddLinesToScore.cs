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
                scores.ValueRW.lines += numLines;

                var score = 0;
                switch(numLines)
                {
                    case 1:
                        score = 100;
                        break;
                    case 2:
                        score = 300;
                        break;
                    case 3:
                        score = 500;
                        break;
                    case 4:
                        score = 800;
                        break;
                }

                scores.ValueRW.score += score * scores.ValueRO.level;
            }

            state.EntityManager.AddComponent<ScoredLinesAdded>(SystemAPI.QueryBuilder()
                .WithAll<DetectedLinesBuffer>()
                .WithNone<ScoredLinesAdded>()
                .Build());
        }
    }
}