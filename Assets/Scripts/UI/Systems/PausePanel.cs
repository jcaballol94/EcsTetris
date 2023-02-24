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
            if (!SystemAPI.TryGetSingletonRW(out RefRW<TimeScale> timeScale)) return;

            var paused = !panel.value.value.activeSelf;
            // Toggle the state of the UI panel
            panel.value.value.SetActive(paused);
            // Set the time scale to 0 when paused
            timeScale.ValueRW.value = paused ? 0f : 1f;
            // Disable the game input when paused
            TetrisInputReader.allowGameInput = !paused;

            state.EntityManager.DestroyEntity(SystemAPI.QueryBuilder().WithAll<TogglePauseTag>().Build());
        }
    }
}