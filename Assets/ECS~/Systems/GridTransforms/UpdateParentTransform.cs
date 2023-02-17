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
    public partial struct UpdateParentTransformSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(Position))]
        public partial struct UpdateParentTransformJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<ParentTransform> parentPosLookup;

            [BurstCompile]
            private void Execute(in Position pos, in OrientationMatrix matrix, in DynamicBuffer<ChildBlockBuffer> children)
            {
                var newValue = new ParentTransform
                {
                    position = pos.value,
                    matrix = matrix.value
                };
                foreach (var child in children)
                {
                    var childPos = parentPosLookup.GetRefRW(child.value, false);
                    childPos.ValueRW = newValue;
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
            var parentPosLookup = SystemAPI.GetComponentLookup<ParentTransform>();
            new UpdateParentTransformJob
            {
                parentPosLookup = parentPosLookup
            }.ScheduleParallel();
        }
    }
}