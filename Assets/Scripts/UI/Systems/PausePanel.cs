using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(ReadPauseInputSystem))]
    public partial struct PausePanelSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PausePanelReference>();
            // This only runs if the toggle pause tag exists
            state.RequireForUpdate<TogglePauseTag>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton(out PausePanelReference panel)) return;

            panel.value.value.SetActive(!panel.value.value.activeSelf);

            state.EntityManager.DestroyEntity(SystemAPI.QueryBuilder().WithAll<TogglePauseTag>().Build());
        }
    }
}