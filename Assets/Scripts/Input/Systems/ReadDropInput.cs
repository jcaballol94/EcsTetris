using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct ReadDropInput : ISystem
    {
        private EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<DropInput>().Build();

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

            var value = input.value.Game.Fall.IsPressed();

            new ReadDropInputJob
            {
                fast = value
            }.Run(m_query);
        }
    }

    [BurstCompile]
    public partial struct ReadDropInputJob : IJobEntity
    {
        public bool fast;

        [BurstCompile]
        private void Execute(ref DropInput input)
        {
            input.fast = fast;
        }
    }
}