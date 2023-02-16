using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct MoveTetriminoSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, movement)
                in SystemAPI.Query<RefRO<InputValues>, TetriminoMovement>()
                .WithChangeFilter<InputValues>())
            {
                if (input.ValueRO.movePressed && input.ValueRO.moveValue != 0)
                    movement.TryMove(new int2(input.ValueRO.moveValue, 0));

                if (input.ValueRO.rotatePressed && input.ValueRO.rotateValue != 0)
                    movement.TryRotate(input.ValueRO.rotateValue);
            }
        }
    }
}