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
            private void Execute(in Transform transform, ref OrientationMatrix matrix)
            {
                matrix.value = OrientationMatrix.CalculateForRotation(transform.orientation);
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