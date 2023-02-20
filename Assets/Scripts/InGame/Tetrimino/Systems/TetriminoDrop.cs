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
    [UpdateInGroup(typeof(MovementSystemGroup))]
    [UpdateAfter(typeof(TetriminoMovementSystem))]
    public partial struct TetriminoDropSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;

            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (movement, dropState, player) in SystemAPI
                .Query<TetriminoMovement, RefRW<DropState>, RefRO<PlayerRef>>())
            {
                var input = state.EntityManager.GetComponentData<InputValues>(player.ValueRO.value);

                var newDropAmount = dropState.ValueRO.currentDrop;
                if (input.drop)
                    newDropAmount += gameData.dropLength;
                else
                    newDropAmount += gameData.fallSpeed * deltaTime * (input.fall ? gameData.fastFallMultiplier : 1f);

                while (newDropAmount >= 1f)
                {
                    movement.TryMove(new int2(0, -1));
                    newDropAmount -= 1f;
                }

                dropState.ValueRW.currentDrop = newDropAmount;
            }
        }
    }
}