using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct GameInputHolderSystem : ISystem, ISystemStartStop
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TetrisGameMode>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnStartRunning(ref SystemState state)
        {
            var input = new TetrisInput();
            input.Enable();
            input.Game.Enable();

            state.EntityManager.AddComponentObject(state.SystemHandle, new GameInputComponent { value = input });
        }

        public void OnStopRunning(ref SystemState state)
        {
            var input = SystemAPI.ManagedAPI.GetComponent<GameInputComponent>(state.SystemHandle);
            input.value.Game.Disable();
            input.value.Disable();
            input.value.Dispose();

            state.EntityManager.RemoveComponent<GameInputComponent>(state.SystemHandle);
        }

        public void OnUpdate(ref SystemState state)
        {
        }
    }
}