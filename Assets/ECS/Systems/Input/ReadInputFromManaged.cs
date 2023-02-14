using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct ReadInputFromManagedSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (reader, managed) in SystemAPI.Query<RefRW<InputReader>, ManagedInput>())
            {
                var newMove = Mathf.RoundToInt(managed.value.Game.Move.ReadValue<float>());
                if (newMove != reader.ValueRO.moveValue)
                {
                    reader.ValueRW.moveValue = newMove;
                    reader.ValueRW.movePressed = true;
                }
                else if (reader.ValueRO.movePressed)
                {
                    reader.ValueRW.movePressed = false;
                }
            }
        }
    }
}