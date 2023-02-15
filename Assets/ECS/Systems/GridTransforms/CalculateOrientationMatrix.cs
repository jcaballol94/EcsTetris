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
    [UpdateInGroup(typeof(MovementSystemGroup), OrderLast = true)]
    public partial struct CalculateOrientationMatrixSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(Transform))]
        public partial struct CalculateOrientationMatrixJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in Transform orientation, ref OrientationMatrix matrix)
            {
                switch (orientation.orientation)
                {
                    case 0:
                        matrix.value = int2x2.identity;
                        break;
                    case 1:
                        matrix.value = new int2x2(0, -1, 1, 0);
                        break;
                    case 2:
                        matrix.value = new int2x2(-1, 0, 0, -1);
                        break;
                    case 3:
                        matrix.value = new int2x2(0, 1, -1, 0);
                        break;
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
            new CalculateOrientationMatrixJob().ScheduleParallel();
        }
    }
}