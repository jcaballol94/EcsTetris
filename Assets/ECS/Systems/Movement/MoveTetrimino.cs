using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(MovementSystemGroup))]
    [UpdateBefore(typeof(RotateTetriminoSystem))]
    public partial struct MoveTetriminoSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, position, matrix, definition, collider)
                in SystemAPI.Query<RefRO<InputValues>, RefRW<Position>, RefRO<OrientationMatrix>, RefRO<TetriminoType>, GridCollider>()
                .WithChangeFilter<InputValues>())
            {
                if (!input.ValueRO.movePressed || input.ValueRO.moveValue == 0)
                    continue;

                // The new desired pos
                var newPos = position.ValueRO.value;
                newPos.x += input.ValueRO.moveValue;

                // Check that the movement is possible
                bool canMove = true;
                ref var blocksDef = ref definition.ValueRO.asset.Value.blocks;
                for (int i = 0; canMove && i < blocksDef.Length; ++i)
                {
                    canMove = collider.IsPositionValid(newPos + math.mul(blocksDef[i], matrix.ValueRO.value)); ;
                }

                if (canMove)
                    position.ValueRW.value = newPos;
            }
        }
    }
}