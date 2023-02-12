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
    [UpdateAfter(typeof(UpdateParentPositionSystem))]
    public partial struct CalculateChildPositionSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(ParentPosition), typeof(LocalPosition))]
        public partial struct CalculateChildPositionJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in ParentPosition parentPos, in LocalPosition localPos, ref Position pos)
            {
                pos.value = parentPos.value + localPos.value;
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