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
            foreach (var (movement, player) in SystemAPI
                .Query<TetriminoMovement, RefRO<PlayerRef>>())
            {
                var input = state.EntityManager.GetComponentData<InputValues>(player.ValueRO.value);

                if (input.move != 0)
                    movement.TryMove(new int2(input.move, 0));

                if (input.rotate != 0)
                    movement.TryRotate(input.rotate);
            }
        }
    }
}