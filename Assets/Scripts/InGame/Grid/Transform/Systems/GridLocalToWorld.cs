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
            private void Execute(in LocalGridTransform local, ref WorldGridTransform world)
            {
                world.value = local.value;
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
                foreach (var child in children)
                {
                    var parent = m_parentLookup.GetRefRW(child.value, false);
                    parent.ValueRW.value = local.value;
                }
            }
        }

        [BurstCompile]
        [WithChangeFilter(typeof(ParentGridTransform))]
        // Passes the current world position to the children
        public partial struct UpdateLocalToWorldTransformJob : IJobEntity
        {

            [BurstCompile]
            private void Execute(in ParentGridTransform parent, in LocalGridTransform local, ref WorldGridTransform trans)
            {
                trans.value = parent.value + local.value;
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
            // Ensure that we have the required components
            state.EntityManager.AddComponent<WorldGridTransform>(SystemAPI.QueryBuilder()
                .WithAll<LocalGridTransform>()
                .WithNone<WorldGridTransform>()
                .Build());

            state.EntityManager.AddComponent<ParentGridTransform>(SystemAPI.QueryBuilder()
                .WithAll<GridParent, LocalGridTransform>()
                .WithNone<ParentGridTransform>()
                .Build());

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