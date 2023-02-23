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
        private GridCollisions.Lookup m_colliderLookup;

        public partial struct TetriminoDropJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<InputValues> inputLookup;
            [ReadOnly] public GridCollisions.Lookup colliderLookup;

            public GameData gameData;
            public float deltaTime;

            private void Execute(ref TetriminoMovement movement, ref DropState dropState, in PlayerCleanupRef player, in GridRef grid)
            {
                var input = inputLookup[player.value];
                var collider = colliderLookup[grid.value];

                // Calculate how much we need to drop
                var newDropAmount = dropState.currentDrop;
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
                    if (dropState.HasMoved(transform))
                    {
                        // If it has moved since the last collision, we store the new position
                        // The tetrimino stays alive (the player can slide before they are placed)
                        dropState.lastCollision = transform;
                    }
                    else
                    {
                        // If it hasn't moved since the last collision, place it
                        // TODO actually remove the tetrimino
                        break;
                    }
                }

                dropState.currentDrop = newDropAmount;
            }
        }
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();

            m_colliderLookup = new GridCollisions.Lookup(ref state, true);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;

            var deltaTime = SystemAPI.Time.DeltaTime;
            var inputLookup = SystemAPI.GetComponentLookup<InputValues>(true);
            m_colliderLookup.Update(ref state);

            state.Dependency = new TetriminoDropJob
            {
                colliderLookup = m_colliderLookup,
                deltaTime = deltaTime,
                gameData = gameData,
                inputLookup = inputLookup
            }.Schedule(state.Dependency);
        }
    }
}