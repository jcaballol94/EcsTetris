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
    [UpdateInGroup(typeof(TetrisGameLogic))]
    [UpdateAfter(typeof(SpawnTetrimino))]
    [UpdateAfter(typeof(PlayerMovement))]
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
                positionLookup = SystemAPI.GetComponentLookup<PositionInGrid>(false),
                localLookup = SystemAPI.GetComponentLookup<LocalBlock>(true)
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithAll(typeof(PositionInGrid))]
    [WithChangeFilter(typeof(PositionInGrid))]
    [WithChangeFilter(typeof(Orientation))]
    public partial struct UpdateLocalBlocksJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<PositionInGrid> positionLookup;
        [ReadOnly] public ComponentLookup<LocalBlock> localLookup;

        [BurstCompile]
        private void Execute(Entity entity, in Orientation orientation, in DynamicBuffer<TetriminoBlockList> blocks)
        {
            var parentPos = positionLookup.GetRefRO(entity);

            foreach (var block in blocks)
            {
                var localPos = localLookup.GetRefRO(block.Value);
                var blockPos = positionLookup.GetRefRW(block.Value, false);
                blockPos.ValueRW.value = parentPos.ValueRO.value + localPos.ValueRO.position;
            }
        }
    }
}