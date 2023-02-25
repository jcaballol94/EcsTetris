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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(ReadInputSystem))]
    [UpdateBefore(typeof(MovementSystemGroup))]
    public partial struct HoldTetriminoSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(HoldTetrimino))]
        public partial struct HoldNewTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            [ReadOnly] public ComponentLookup<TetriminoData> dataLookup;

            [BurstCompile]
            private void Execute(Entity entity, in InputValues input, in TetriminoRef tetrimino)
            {
                if (!input.hold) return;

                var data = dataLookup[tetrimino.value];

                // Store the tetrimino as hold and destroy the real one
                ecb.AddComponent(entity, new HoldTetrimino { data = data });
                ecb.DestroyEntity(tetrimino.value);
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
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var lookup = SystemAPI.GetComponentLookup<TetriminoData>(true);

            new HoldNewTetriminoJob
            {
                ecb = ecb,
                dataLookup = lookup
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}