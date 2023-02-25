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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnPreviewBlocksSystem))]
    public partial struct UpdateHoldPreviewSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(HoldTetrimino))]
        public partial struct UpdateHoldPreviewJob : IJobEntity
        {
            public ComponentLookup<TetriminoData> dataLookup;

            [BurstCompile]
            private void Execute(in HoldTetrimino hold, in HoldPreview preview)
            {
                var data = dataLookup.GetRefRW(preview.value, false);

                data.ValueRW = hold.data;
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
            var lookup = SystemAPI.GetComponentLookup<TetriminoData>();
            new UpdateHoldPreviewJob
            {
                dataLookup = lookup
            }.Schedule();
        }
    }
}