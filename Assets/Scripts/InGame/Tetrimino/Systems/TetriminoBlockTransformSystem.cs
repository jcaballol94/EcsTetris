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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderLast = true)]
    public partial struct TetriminoBlockTransformSystem : ISystem
    {
        [BurstCompile]
        [WithChangeFilter(typeof(TetriminoPosition))]
        // Passes the current world position to the children
        public partial struct UpdateChildrenTransformJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<BlockPosition> m_blockLookup;

            //[BurstCompile]
            private void Execute(in TetriminoTransformAspect transform, in TetriminoData definition, in DynamicBuffer<TetriminoBlockBuffer> blocks)
            {
                ref var blockDefinitions = ref definition.blocks;

                for (int i = 0; i < blockDefinitions.Length; ++i)
                {
                    Debug.Log($"Block {i}, definition {blockDefinitions[i]}");
                    var blockPos = m_blockLookup.GetRefRW(blocks[i].value, false);
                    blockPos.ValueRW.position = transform.TransformPoint(blockDefinitions[i]);
                }
            }
        }

        [BurstCompile]
        [WithChangeFilter(typeof(TetriminoPosition))]
        public partial struct UpdateOrientationMatrixJob : IJobEntity
        {

            [BurstCompile]
            private void Execute(in TetriminoPosition pos, ref TetriminoOrientationMatrix matrix)
            {
                matrix.value = TetriminoOrientationMatrix.GetMatrixForOrientation(pos.orientation);
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
            // All tetriminos with a transform also need to compute the matrix
            state.EntityManager.AddComponent<TetriminoOrientationMatrix>(SystemAPI.QueryBuilder()
                .WithAll<TetriminoPosition>()
                .WithNone<TetriminoOrientationMatrix>()
                .Build());

            // Update the matrices first
            new UpdateOrientationMatrixJob().ScheduleParallel();

            // Update the parent transforms
            var blockLookup = SystemAPI.GetComponentLookup<BlockPosition>();
            new UpdateChildrenTransformJob
            {
                m_blockLookup = blockLookup
            }.ScheduleParallel();
        }
    }
}