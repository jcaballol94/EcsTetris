using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct ReadMoveInput : ISystem
    {
        private EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<MoveInput>().Build();

            state.RequireForUpdate(m_query);
            state.RequireForUpdate<GameInputComponent>();
            state.RequireForUpdate<RepeatMoveData>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton(out GameInputComponent input))
                return;

            if (!SystemAPI.TryGetSingleton(out RepeatMoveData data))
                return;

            var value = Mathf.RoundToInt(input.value.Game.Move.ReadValue<float>());

            var time = SystemAPI.Time.DeltaTime;

            new ReadMoveInputJob
            {
                newValue = value,
                deltaTime = time,
                data = data
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct ReadMoveInputJob : IJobEntity
    {
        public int newValue;
        public float deltaTime;
        public RepeatMoveData data;

        [BurstCompile]
        private void Execute(ref MoveInput input)
        {
            input.changed = input.value != newValue;
            input.value = newValue;

            if (input.changed)
            {
                // If we have changed, restart everything
                input.elapsedTime = 0f;
                input.repeating = false;
            }
            else
            {
                // See if we need to repeat again
                input.elapsedTime += deltaTime;
                if (input.elapsedTime > (input.repeating ? data.time : data.startDelay))
                {
                    input.elapsedTime = 0f;
                    input.repeating = true;
                    input.changed = true;
                }
            }
        }
    }
}