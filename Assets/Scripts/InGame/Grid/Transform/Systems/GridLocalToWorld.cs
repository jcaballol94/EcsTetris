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
    [UpdateInGroup(typeof(GridTransformSystemGroup))]
    public partial struct GridLocalToWorldSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(LocalGridPosition))]
        public partial struct CopyLocalToWorldJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in LocalGridPosition local, ref WorldGridPosition world)
            {
                world.value = local.value;
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
            state.EntityManager.AddComponent<WorldGridPosition>(SystemAPI.QueryBuilder()
                .WithAll<LocalGridPosition>()
                .WithNone<WorldGridPosition>()
                .Build());

            // For entities without a parent, the word is a copy of the local
            new CopyLocalToWorldJob().ScheduleParallel();
        }
    }
}