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
            state.RequireForUpdate<PlaceTetriminoEvent>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlaceTetriminoEvent> placeEvents)) return;

            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (movement, dropState, player, grid, entity) in SystemAPI
                .Query<TetriminoMovement, RefRW<DropState>, RefRO<PlayerRef>, RefRO<GridRef>>()
                .WithEntityAccess())
            {
                var input = state.EntityManager.GetComponentData<InputValues>(player.ValueRO.value);
                var collider = state.EntityManager.GetAspectRO<GridCollisions>(grid.ValueRO.value);

                var newDropAmount = dropState.ValueRO.currentDrop;
                if (input.drop)
                    newDropAmount += gameData.dropLength;
                else
                    newDropAmount += gameData.fallSpeed * deltaTime * (input.fall ? gameData.fastFallMultiplier : 1f);

                while (newDropAmount >= 1f)
                {
                    newDropAmount -= 1f;
                    if (!movement.TryMove(new int2(0, -1), collider))
                    {
                        // If it hasn't moved since the last collision, place it
                        var transform = movement.LocalTransform;
                        if (dropState.ValueRO.HasMoved(transform))
                            dropState.ValueRW.lastCollision = transform;
                        else
                        {
                            placeEvents.Add(new PlaceTetriminoEvent { tetrimino = entity });
                            break;
                        }
                    }
                }

                dropState.ValueRW.currentDrop = newDropAmount;
            }
        }
    }
}