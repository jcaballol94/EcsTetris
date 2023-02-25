using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnTetriminoSystem))]
    public partial struct GameOverPanelSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameOverTag>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!GameObjectReferences.Instance.TryGetObject("GameOver", out var panel)) return;

            if (!panel.activeSelf)
                panel.SetActive(true);
        }
    }
}