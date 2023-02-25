using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial struct UpdatePreviewBlocksSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(TetriminoData))]
        public partial struct UpdatePreviewBlocksJob : IJobEntity
        {
            public ComponentLookup<BlockPosition> positionLookup;
            public ComponentLookup<URPMaterialPropertyBaseColor> colorLookup;

            [BurstCompile]
            private void Execute(in TetriminoData data, in DynamicBuffer<PreviewBlocks> blocks)
            {
                for (int i = 0; i < blocks.Length; ++i)
                {
                    var position = positionLookup.GetRefRW(blocks[i].value, false);
                    position.ValueRW.position = data.blocks[i];

                    var color = colorLookup.GetRefRW(blocks[i].value, false);
                    color.ValueRW.Value = data.color;
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
            var positionLookup = SystemAPI.GetComponentLookup<BlockPosition>();
            var colorLookup = SystemAPI.GetComponentLookup<URPMaterialPropertyBaseColor>();

            new UpdatePreviewBlocksJob
            {
                colorLookup = colorLookup,
                positionLookup = positionLookup
            }.Schedule();
        }
    }
}