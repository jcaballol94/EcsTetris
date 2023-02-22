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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateGridCollisionsSystem))]
    public partial struct DetectLinesSystem : ISystem
    {
        [BurstCompile]
        public partial struct DetectLinesJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in GridBounds bounds, in DynamicBuffer<GridCellData> cells, ref DynamicBuffer<RemoveLineEvent> events)
            {
                var lineSize = bounds.size.x;
                for (int i = 0; i < cells.Length; i += lineSize)
                {
                    var line = true;
                    for (int j = 0; line && j < lineSize; ++j)
                    {
                        line = !cells[i * lineSize + j].available;
                    }
                    if (line)
                    {
                        events.Add(new RemoveLineEvent { lineY = (i / lineSize) });
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
            new DetectLinesJob().ScheduleParallel();
        }
    }
}