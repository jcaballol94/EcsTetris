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
    public partial struct TetriminoMovementSystem : ISystem
    {
        public partial struct TetriminoMovementJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<InputValues> inputLookup;
            [ReadOnly] public GridCollisions.Lookup colliderLookup;

            private void Execute(TetriminoMovement movement, in PlayerCleanupRef player, in GridRef grid)
            {
                var input = inputLookup[player.value];
                var collider = colliderLookup[grid.value];

                if (input.move != 0)
                    movement.TryMove(new int2(input.move, 0), collider);

                if (input.rotate != 0)
                    movement.TryRotate(input.rotate, collider);
            }
        }
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var inputLookup = SystemAPI.GetComponentLookup<InputValues>();
            var colliderLookup = new GridCollisions.Lookup(ref state, true);

            new TetriminoMovementJob
            {
                colliderLookup = colliderLookup,
                inputLookup = inputLookup
            }.ScheduleParallel();
        }
    }
}