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

                // Mark that we already swapped to prevent eternal swapping
                ecb.AddComponent<UsingOnHold>(entity);
            }
        }

        [BurstCompile]
        [WithNone(typeof(UsingOnHold))]
        public partial struct SwapHoldTetriminoJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            public ComponentLookup<TetriminoData> dataLookup;
            public ComponentLookup<TetriminoPosition> positionLookup;

            public GameData gameData;

            [BurstCompile]
            private void Execute(Entity entity, in InputValues input, in TetriminoRef tetrimino, ref HoldTetrimino hold)
            {
                if (!input.hold) return;

                var data = dataLookup.GetRefRW(tetrimino.value, false);

                // Swap the tetrimino type
                var aux = hold.data;
                hold.data = data.ValueRO;
                data.ValueRW = aux;

                // Put the tetrimino back at the top
                var position = positionLookup.GetRefRW(tetrimino.value, false);
                position.ValueRW.position = gameData.spawnPosition;
                position.ValueRW.orientation = 0;

                // Mark that we already swapped to prevent eternal swapping
                ecb.AddComponent<UsingOnHold>(entity);
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var tetriminoLookup = SystemAPI.GetComponentLookup<TetriminoData>(false);
            var positionLookup = SystemAPI.GetComponentLookup<TetriminoPosition>(false);

            state.Dependency.Complete();

            new HoldNewTetriminoJob
            {
                ecb = ecb,
                dataLookup = tetriminoLookup
            }.Run();

            new SwapHoldTetriminoJob
            {
                ecb = ecb,
                dataLookup = tetriminoLookup,
                gameData = gameData,
                positionLookup = positionLookup
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}