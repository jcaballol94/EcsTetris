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
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (reader, settings, managed) in SystemAPI.Query<RefRW<InputReader>, GameSettings, ManagedInput>())
            {
                var newMove = Mathf.RoundToInt(managed.value.Game.Move.ReadValue<float>());
                if (newMove != reader.ValueRO.moveValue)
                {
                    reader.ValueRW.moveValue = newMove;
                    reader.ValueRW.movePressed = true;
                    reader.ValueRW.timeSinceLastMove = 0f;
                    reader.ValueRW.repeatingMove = false;
                }
                else 
                {
                    reader.ValueRW.timeSinceLastMove += deltaTime;
                    if (reader.ValueRO.timeSinceLastMove >= (reader.ValueRO.repeatingMove ? settings.moveRepeatRatio : settings.moveRepeatDelay))
                    {
                        reader.ValueRW.movePressed = true;
                        reader.ValueRW.timeSinceLastMove = 0f;
                        reader.ValueRW.repeatingMove = true;
                    }
                    else if (reader.ValueRO.movePressed)
                    {
                        reader.ValueRW.movePressed = false;
                    }
                }

                var newRotate = Mathf.RoundToInt(managed.value.Game.Rotate.ReadValue<float>());
                if (newRotate != reader.ValueRO.rotateValue)
                {
                    reader.ValueRW.rotateValue = newRotate;
                    reader.ValueRW.rotatePressed = true;
                }
                else if (reader.ValueRO.rotatePressed)
                {
                    reader.ValueRW.rotatePressed = false;
                }

                reader.ValueRW.fallFast = managed.value.Game.Fall.IsPressed();
                reader.ValueRW.prevDrop = reader.ValueRO.drop;
                reader.ValueRW.drop = managed.value.Game.Drop.IsPressed();

            }
        }
    }
}