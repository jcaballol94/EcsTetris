using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
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

                // Calculate how much we need to drop
                var newDropAmount = dropState.ValueRO.currentDrop;
                if (input.drop)
                    newDropAmount += gameData.dropLength;
                else
                    newDropAmount += gameData.fallSpeed * deltaTime * (input.fall ? gameData.fastFallMultiplier : 1f);

                // Drop 1 cell at a time
                while (newDropAmount >= 1f)
                {
                    newDropAmount -= 1f;

                    // Successful movement, continue dropping
                    if (movement.TryMove(new int2(0, -1), collider))
                        continue;

                    var transform = movement.LocalTransform;
                    if (dropState.ValueRO.HasMoved(transform))
                    {
                        // If it has moved since the last collision, we store the new position
                        // The tetrimino stays alive (the player can slide before they are placed)
                        dropState.ValueRW.lastCollision = transform;
                    }
                    else
                    {
                        // If it hasn't moved since the last collision, place it
                        placeEvents.Add(new PlaceTetriminoEvent { tetrimino = entity });
                        break;
                    }
                }

                dropState.ValueRW.currentDrop = newDropAmount;
            }
        }
    }
}