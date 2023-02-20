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
    public partial struct TetriminoMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (movement, player, grid) in SystemAPI
                .Query<TetriminoMovement, RefRO<PlayerRef>, RefRO<GridRef>>())
            {
                var input = state.EntityManager.GetComponentData<InputValues>(player.ValueRO.value);
                var collider = state.EntityManager.GetAspectRO<GridCollisions>(grid.ValueRO.value);

                if (input.move != 0)
                    movement.TryMove(new int2(input.move, 0), collider);

                if (input.rotate != 0)
                    movement.TryRotate(input.rotate, collider);
            }
        }
    }
}