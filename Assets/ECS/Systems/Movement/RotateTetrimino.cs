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
            foreach (var (input, position, definition, collider)
                in SystemAPI.Query<RefRO<InputValues>, RefRW<Transform>, RefRO<TetriminoType>, GridCollider>()
                .WithChangeFilter<InputValues>())
            {
                if (!input.ValueRO.rotatePressed || input.ValueRO.rotateValue == 0)
                    continue;

                var oldPosition = position.ValueRO;
                var newPosition = oldPosition;
                newPosition.orientation = oldPosition.orientation + input.ValueRO.rotateValue;
                if (newPosition.orientation < 0)
                    newPosition.orientation += 4;
                if (newPosition.orientation > 3)
                    newPosition.orientation -= 4;

                var newMatrix = OrientationMatrix.CalculateForRotation(newPosition.orientation);

                // Check all the possible offsets
                ref var offsets = ref definition.ValueRO.asset.Value.rotationOffsets;
                ref var blocks = ref definition.ValueRO.asset.Value.blocks;
                bool canMove = false;
                for (int i = 0; !canMove && i < offsets.Length; ++i)
                {
                    newPosition.position = oldPosition.position + 
                        offsets[oldPosition.orientation].offsets[i] - offsets[newPosition.orientation].offsets[i];

                    // Check all the blocks
                    canMove = true;
                    for (int j = 0; canMove && j < blocks.Length; ++j)
                    {
                        canMove = collider.IsPositionValid(newPosition.position + math.mul(blocks[j], newMatrix));
                    }
                }

                // If possible, apply the move
                if (canMove)
                    position.ValueRW = newPosition;
            }
        }
    }
}