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
                positionLookup = SystemAPI.GetComponentLookup<PositionInGrid>(false),
                localPositionLookup = SystemAPI.GetComponentLookup<LocalBlock>(true)
            }.Run();
        }
    }

    [BurstCompile]
    [WithChangeFilter(typeof(PositionInGrid))]
    [WithAll(typeof(PositionInGrid))]
    public partial struct UpdateLocalBlocksJob : IJobEntity
    {
        public ComponentLookup<PositionInGrid> positionLookup;
        [ReadOnly] public ComponentLookup<LocalBlock> localPositionLookup;

        [BurstCompile]
        private void Execute(Entity parent, in DynamicBuffer<TetriminoBlockList> children)
        {
            var parentPos = positionLookup.GetRefRO(parent);

            foreach (var child in children)
            {
                var localPos = localPositionLookup.GetRefRO(child.Value);
                var pos = positionLookup.GetRefRW(child.Value, false);

                pos.ValueRW.value = parentPos.ValueRO.value + localPos.ValueRO.position;
            }
        }
    }
}