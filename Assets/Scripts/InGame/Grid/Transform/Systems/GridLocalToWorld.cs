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
    [UpdateInGroup(typeof(GridTransformSystemGroup))]
    public partial struct GridLocalToWorldSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(GridParent))]
        [WithChangeFilter(typeof(LocalGridTransform))]
        [WithNone(typeof(ParentGridTransform))]
        public partial struct CopyLocalToWorldJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in LocalGridTransform local, in GridOrientationMatrix matrix, ref WorldGridTransform world)
            {
                world.position = local.position;
                world.matrix = matrix.value;
            }
        }

        [BurstCompile]
        [WithChangeFilter(typeof(WorldGridTransform))]
        // Passes the current world position to the children
        public partial struct UpdateChildrenTransformJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<ParentGridTransform> m_parentLookup;

            [BurstCompile]
            private void Execute(in WorldGridTransform local, in DynamicBuffer<GridChildren> children)
            {
                var newValue = new ParentGridTransform
                {
                    position = local.position,
                    matrix = local.matrix
                };

                foreach (var child in children)
                {
                    var parent = m_parentLookup.GetRefRW(child.value, false);
                    parent.ValueRW = newValue;
                }
            }
        }

        [BurstCompile]
        [WithChangeFilter(typeof(ParentGridTransform), typeof(LocalGridTransform))]
        public partial struct UpdateLocalToWorldTransformJob : IJobEntity
        {

            [BurstCompile]
            private void Execute(in ParentGridTransform parent, in LocalGridTransform local, in GridOrientationMatrix matrix, ref WorldGridTransform trans)
            {
                trans.position = parent.TransformPoint(local.position);
                trans.matrix = math.mul(matrix.value, parent.matrix);
            }
        }

        [BurstCompile]
        [WithChangeFilter(typeof(LocalGridTransform))]
        public partial struct UpdateOrientationMatrixJob : IJobEntity
        {

            [BurstCompile]
            private void Execute(in LocalGridTransform local, ref GridOrientationMatrix matrix)
            {
                matrix.value = GridOrientationMatrix.GetMatrixForOrientation(local.orientation);
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
            // All entities with a local transform also need a world transform
            state.EntityManager.AddComponent<WorldGridTransform>(SystemAPI.QueryBuilder()
                .WithAll<LocalGridTransform>()
                .WithNone<WorldGridTransform>()
                .Build());

            // All objects with a parent need a copy of the parent's transform
            state.EntityManager.AddComponent<ParentGridTransform>(SystemAPI.QueryBuilder()
                .WithAll<GridParent, LocalGridTransform>()
                .WithNone<ParentGridTransform>()
                .Build());

            // Objects without a parent don't need the parent transform anymore
            state.EntityManager.RemoveComponent<ParentGridTransform>(SystemAPI.QueryBuilder()
                .WithAll<ParentGridTransform>()
                .WithNone<GridParent>()
                .Build());

            // All objects with a transform also need to compute the matrix
            state.EntityManager.AddComponent<GridOrientationMatrix>(SystemAPI.QueryBuilder()
                .WithAll<LocalGridTransform>()
                .WithNone<GridOrientationMatrix>()
                .Build());

            // Update the matrices first
            new UpdateOrientationMatrixJob().ScheduleParallel();

            // For entities without a parent, the world is a copy of the local
            new CopyLocalToWorldJob().ScheduleParallel();

            // Update the parent transforms
            var parentLookup = SystemAPI.GetComponentLookup<ParentGridTransform>();
            new UpdateChildrenTransformJob
            {
                m_parentLookup = parentLookup
            }.ScheduleParallel();

            // Update the entities that do have a parent
            new UpdateLocalToWorldTransformJob().ScheduleParallel();
        }
    }
}