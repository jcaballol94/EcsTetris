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
    [UpdateInGroup(typeof(GridTransformsSystemGroup))]
    [UpdateAfter(typeof(UpdateParentTransformSystem))]
    public partial struct CalculateChildPositionSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(ParentTransform), typeof(LocalPosition))]
        public partial struct CalculateChildPositionJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in ParentTransform parentPos, in LocalPosition localPos, ref Transform pos)
            {
                pos.position = parentPos.position + math.mul(localPos.value, parentPos.matrix);
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
            new CalculateChildPositionJob().ScheduleParallel();
        }
    }
}