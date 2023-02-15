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

            foreach (var (transform, matrix, fallStatus, definition, collider, settings)
                in SystemAPI.Query<RefRW<Transform>, RefRO<OrientationMatrix>, RefRW<FallStatus>, RefRO<TetriminoType>, GridCollider, GameSettings>())
            {
                var newTransform = transform.ValueRO;
                var newStatus = fallStatus.ValueRO;

                newStatus.timeToFall -= deltaTime;
                while (newStatus.timeToFall < 0f)
                {
                    newStatus.timeToFall += 1f / settings.fallSpeed;
                    var newPos = newTransform.position;
                    newPos.y--;

                    ref var blocks = ref definition.ValueRO.asset.Value.blocks;
                    var canMove = true;
                    for (int i = 0; canMove && i < blocks.Length; ++i)
                    {
                        canMove = collider.IsPositionValid(newPos + math.mul(blocks[i], matrix.ValueRO.value));
                    }

                    if (canMove)
                        newTransform.position = newPos;
                }

                fallStatus.ValueRW = newStatus;
                transform.ValueRW = newTransform;
            }
        }
    }
}