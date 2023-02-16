using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(BlockPlacingSystemGroup))]
    public partial struct FallTetriminoSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (movement, fallStatus, settings)
                in SystemAPI.Query<TetriminoMovement, RefRW<FallStatus>, GameSettings>())
            {
                var newStatus = fallStatus.ValueRO;

                newStatus.timeToFall -= deltaTime;
                var fallTime = 1f / settings.fallSpeed;

                while (newStatus.timeToFall < 0f)
                {
                    newStatus.timeToFall += fallTime;

                    movement.TryMove(new int2(0, -1));
                }

                fallStatus.ValueRW = newStatus;
            }
        }
    }
}