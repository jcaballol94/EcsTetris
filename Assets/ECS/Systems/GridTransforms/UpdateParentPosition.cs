using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GridTransformsSystemGroup))]
    [UpdateAfter(typeof(AddParentPositionComponentSystem))]
    public partial struct UpdateParentPositionSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(Position))]
        public partial struct UpdateParentPositionJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<ParentPosition> parentPosLookup;

            [BurstCompile]
            private void Execute(in Position pos, in DynamicBuffer<ChildBlockBuffer> children)
            {
                foreach (var child in children)
                {
                    var childPos = parentPosLookup.GetRefRW(child.value, false);
                    childPos.ValueRW.value = pos.value;
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
            var parentPosLookup = SystemAPI.GetComponentLookup<ParentPosition>();
            new UpdateParentPositionJob
            {
                parentPosLookup = parentPosLookup
            }.ScheduleParallel();
        }
    }
}