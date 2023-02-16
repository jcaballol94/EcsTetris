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
    public partial struct RotateTetriminoSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, position, orientation, definition, collider)
                in SystemAPI.Query<RefRO<InputValues>, RefRW<Position>, RefRW<Orientation>, RefRO<TetriminoType>, GridCollider>()
                .WithChangeFilter<InputValues>())
            {
                if (!input.ValueRO.rotatePressed || input.ValueRO.rotateValue == 0)
                    continue;

                var oldPosition = position.ValueRO.value;
                var oldOrientation = orientation.ValueRO.value;
                var newPosition = oldPosition;
                var newOrientation = oldOrientation + input.ValueRO.rotateValue;
                if (newOrientation < 0)
                    newOrientation += 4;
                if (newOrientation > 3)
                    newOrientation -= 4;

                var newMatrix = OrientationMatrix.CalculateForRotation(newOrientation);

                // Check all the possible offsets
                ref var offsets = ref definition.ValueRO.asset.Value.rotationOffsets;
                ref var blocks = ref definition.ValueRO.asset.Value.blocks;
                bool canMove = false;
                for (int i = 0; !canMove && i < offsets.Length; ++i)
                {
                    newPosition = oldPosition + 
                        offsets[oldOrientation].offsets[i] - offsets[newOrientation].offsets[i];

                    // Check all the blocks
                    canMove = true;
                    for (int j = 0; canMove && j < blocks.Length; ++j)
                    {
                        canMove = collider.IsPositionValid(newPosition + math.mul(blocks[j], newMatrix));
                    }
                }

                // If possible, apply the move
                if (canMove)
                {
                    position.ValueRW.value = newPosition;
                    orientation.ValueRW.value = newOrientation;
                }
            }
        }
    }
}