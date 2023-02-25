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
    [UpdateBefore(typeof(SpawnTetriminoSystem))]
    public partial struct UpdateNextPreviewSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(NextTetrimino))]
        public partial struct UpdateNextPreviewJob : IJobEntity
        {
            public ComponentLookup<TetriminoData> dataLookup;

            [BurstCompile]
            private void Execute(in NextTetrimino next, in NextPreview preview)
            {
                var data = dataLookup.GetRefRW(preview.value, false);

                data.ValueRW = next.data;
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
            new UpdateNextPreviewJob
            {
                dataLookup = lookup
            }.Schedule();
        }
    }
}