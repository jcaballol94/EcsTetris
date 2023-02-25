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
            // This only runs if the toggle pause tag exists
            state.RequireForUpdate<TogglePauseTag>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonRW(out RefRW<TimeScale> timeScale)) return;
            if (!GameObjectReferences.Instance.TryGetObject("PausePanel", out var panel)) return;

            var paused = !panel.activeSelf;
            // Toggle the state of the UI panel
            panel.SetActive(paused);
            // Set the time scale to 0 when paused
            timeScale.ValueRW.value = paused ? 0f : 1f;
            // Disable the game input when paused
            foreach (var inputRead in SystemAPI.Query<TetrisInputReader>())
                inputRead.allowGameInput = !paused;

            state.EntityManager.DestroyEntity(SystemAPI.QueryBuilder().WithAll<TogglePauseTag>().Build());
        }
    }
}