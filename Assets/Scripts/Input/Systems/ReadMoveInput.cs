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
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton(out GameInputComponent input))
                return;

            var value = Mathf.RoundToInt(input.value.Game.Move.ReadValue<float>());

            new ReadMoveInputJob
            {
                newValue = value
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct ReadMoveInputJob : IJobEntity
    {
        public int newValue;

        [BurstCompile]
        private void Execute(ref MoveInput input)
        {
            input.changed = input.value != newValue;
            input.value = newValue;
        }
    }
}