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
            foreach (var (input, position)
                in SystemAPI.Query<RefRO<InputValues>, RefRW<Position>>()
                .WithChangeFilter<InputValues>())
            {
                if (!input.ValueRO.movePressed || input.ValueRO.moveValue == 0)
                    continue;

                var newPos = position.ValueRO.value;
                newPos.x += input.ValueRO.moveValue;

                position.ValueRW.value = newPos;
            }
        }
    }
}