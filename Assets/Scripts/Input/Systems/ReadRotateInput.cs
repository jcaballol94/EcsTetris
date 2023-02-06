using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct ReadRotateInput : ISystem
    {
        private EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<RotateInput>().Build();

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

            var value = Mathf.RoundToInt(input.value.Game.Rotate.ReadValue<float>());

            new ReadRotateInputJob
            {
                newValue = value
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct ReadRotateInputJob : IJobEntity
    {
        public int newValue;

        [BurstCompile]
        private void Execute(ref RotateInput input)
        {
            input.changed = input.value != newValue;
            input.value = newValue;
        }
    }
}