using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public partial class ReadInput : SystemBase
    {
        private TetrisInput inputActions;

        protected override void OnCreate()
        {
            inputActions = new TetrisInput();
            inputActions.Enable();
            EntityManager.AddComponentData(SystemHandle, new RotateInput { value = 0, changed = false });
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            inputActions.Game.Enable();
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            inputActions.Game.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            inputActions.Disable();
            inputActions.Dispose();
            inputActions = null;
        }

        protected override void OnUpdate()
        {
            var rotate = SystemAPI.GetComponentRW<RotateInput>(SystemHandle);
            var newRotate = Mathf.RoundToInt(inputActions.Game.Rotate.ReadValue<float>());
            if (rotate.ValueRO.value != newRotate)
            {
                rotate.ValueRW.value = newRotate;
                rotate.ValueRW.changed = true;
            }
            else if (rotate.ValueRO.changed)
            {
                rotate.ValueRW.changed = false;
            }
        }
    }
}