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
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(ReadInputSystem))]
    public partial struct ReadPauseInputSystem : ISystem
    {
        private EntityArchetype m_togglePauseArchetype;
        
        public void OnCreate(ref SystemState state)
        {
            m_togglePauseArchetype = state.EntityManager.CreateArchetype(typeof(TogglePauseTag));
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool togglePause = false;

            foreach(var input in SystemAPI.Query<RefRO<InputValues>>())
            {
                if (input.ValueRO.pause)
                {
                    togglePause = true;
                    break;
                }
            }

            if (togglePause)
            {
                state.EntityManager.CreateEntity(m_togglePauseArchetype);
            }
        }
    }
}