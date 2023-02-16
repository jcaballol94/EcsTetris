using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct TetriminoMovement : IAspect
    {
        private readonly RefRW<Position> m_position;
        private readonly RefRW<Orientation> m_orientation;

        private readonly RefRO<TetriminoType> m_type;
        [Optional] private readonly RefRO<OrientationMatrix> m_matrix;

        [ReadOnly] private readonly GridCollider m_collider;

        public bool TryMove(int2 delta)
        {
            var newPosition = m_position.ValueRO.value + delta;

            if (m_matrix.IsValid)
                return TrySetPosition(newPosition, m_matrix.ValueRO.value);
            else
                return TrySetPosition(newPosition, OrientationMatrix.CalculateForRotation(m_orientation.ValueRO.value));
        }

        public bool TryRotate(int delta)
        {
            var newRotation = m_orientation.ValueRO.value;
            newRotation += delta;
            if (newRotation > 3)
                newRotation -= 4;
            if (newRotation < 0)
                newRotation += 4;

            return TrySetRotation(newRotation, OrientationMatrix.CalculateForRotation(newRotation));
        }

        private bool IsPositionAndTransformValid(int2 position, int2x2 matrix)
        {
            ref var blocks = ref m_type.ValueRO.asset.Value.blocks;
            for (int i = 0; i < blocks.Length; ++i)
            {
                if (!m_collider.IsPositionValid(position + math.mul(blocks[i], matrix)))
                    return false;
            }

            return true;
        }

        private bool TrySetPosition(int2 newPosition, int2x2 matrix)
        {
            if (IsPositionAndTransformValid(newPosition, matrix))
            {
                m_position.ValueRW.value = newPosition;
                return true;
            }

            return false;
        }

        private bool TrySetPositionAndRotation(int2 newPosition, int newRotation, int2x2 newMatrix)
        {
            if (IsPositionAndTransformValid(newPosition, newMatrix))
            {
                m_position.ValueRW.value = newPosition;
                m_orientation.ValueRW.value = newRotation;
                return true;
            }

            return false;
        }

        private bool TrySetRotation(int newRotation, int2x2 newMatrix)
        {
            var position = m_position.ValueRO.value;
            ref var prevOffsets = ref m_type.ValueRO.asset.Value.rotationOffsets[m_orientation.ValueRO.value].offsets;
            ref var newOffsets = ref m_type.ValueRO.asset.Value.rotationOffsets[newRotation].offsets;

            for (int i = 0; i < prevOffsets.Length; i++)
            {
                var newPosition = position + prevOffsets[i] - newOffsets[i];
                if (TrySetPositionAndRotation(newPosition, newRotation, newMatrix))
                    return true;
            }

            return false;
        }
    }
}
