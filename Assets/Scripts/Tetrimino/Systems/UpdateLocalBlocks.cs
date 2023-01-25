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
    [UpdateAfter(typeof(SpawnTetrimino))]
    [UpdateBefore(typeof(GridToWorldSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct UpdateLocalBlocks : ISystem
    {
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
            new UpdateLocalBlocksJob
            {
                positionLookup = SystemAPI.GetComponentLookup<PositionInGrid>(false)
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithAll(typeof(PositionInGrid))]
    public partial struct UpdateLocalBlocksJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<PositionInGrid> positionLookup;

        [BurstCompile]
        private void Execute(Entity entity, in LocalBlock localPos, in ParentTetrimino parent)
        {
            var parentPos = positionLookup.GetRefRO(parent.value);
            var pos = positionLookup.GetRefRW(entity, false);

            pos.ValueRW.value = parentPos.ValueRO.value + localPos.position;
        }
    }
}