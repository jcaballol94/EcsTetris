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

            var system = state.World.GetExistingSystemManaged<EndVariableRateSimulationEntityCommandBufferSystem>();
            var ecb = system.CreateCommandBuffer();

            foreach (var (input, movement, fallStatus, settings, entity)
                in SystemAPI.Query<RefRO<InputValues>, TetriminoMovement, RefRW<FallStatus>, GameSettings>()
                .WithEntityAccess())
            {
                var newStatus = fallStatus.ValueRO;

                newStatus.timeToFall += deltaTime;

                var speed = settings.fallSpeed;
                if (input.ValueRO.fallFast)
                    speed *= settings.fastFallMultiplier;

                var fallTime = 1f / speed;

                if (input.ValueRO.drop)
                    fallTime = deltaTime / 40f;

                while (newStatus.timeToFall > fallTime)
                {
                    newStatus.timeToFall -= fallTime;

                    if (!movement.TryMove(new int2(0, -1)))
                    {
                        // If we couldn't move, and we already had failed once, place the entity
                        if (newStatus.fallFailed)
                            ecb.DestroyEntity(entity);
                        else
                            newStatus.fallFailed = true;
                    }
                }

                fallStatus.ValueRW = newStatus;
            }
        }
    }
}