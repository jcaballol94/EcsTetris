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
        private readonly RefRW<LocalGridTransform> m_localTransform;
        [Optional] private readonly RefRW<GridOrientationMatrix> m_matrix;

        private readonly RefRO<TetriminoData> m_type;

        public bool TryMove(int2 delta, GridCollisions collider)
        {
            var newPosition = m_localTransform.ValueRO.position + delta;

            if (m_matrix.IsValid)
                return TrySetPosition(newPosition, m_matrix.ValueRO.value, collider);
            else
                return TrySetPosition(newPosition, GridOrientationMatrix.GetMatrixForOrientation(m_localTransform.ValueRO.orientation), collider);
        }

        public bool TryRotate(int delta, GridCollisions collider)
        {
            var newRotation = m_localTransform.ValueRO.orientation;
            newRotation += delta;
            if (newRotation > 3)
                newRotation -= 4;
            if (newRotation < 0)
                newRotation += 4;

            return TrySetRotation(newRotation, GridOrientationMatrix.GetMatrixForOrientation(newRotation), collider);
        }

        private bool IsPositionAndTransformValid(int2 position, int2x2 matrix, GridCollisions collider)
        {
            ref var blocks = ref m_type.ValueRO.asset.Value.blocks;
            for (int i = 0; i < blocks.Length; ++i)
            {
                if (!collider.IsPositionValid(position + math.mul(blocks[i], matrix)))
                    return false;
            }

            return true;
        }

        private bool TrySetPosition(int2 newPosition, int2x2 matrix, GridCollisions collider)
        {
            if (IsPositionAndTransformValid(newPosition, matrix, collider))
            {
                m_localTransform.ValueRW.position = newPosition;
                return true;
            }

            return false;
        }

        private bool TrySetPositionAndRotation(int2 newPosition, int newRotation, int2x2 newMatrix, GridCollisions collider)
        {
            if (IsPositionAndTransformValid(newPosition, newMatrix, collider))
            {
                m_localTransform.ValueRW.position = newPosition;
                m_localTransform.ValueRW.orientation = newRotation;

                if (m_matrix.IsValid)
                    m_matrix.ValueRW.value = newMatrix;

                return true;
            }

            return false;
        }

        private bool TrySetRotation(int newRotation, int2x2 newMatrix, GridCollisions collider)
        {
            var position = m_localTransform.ValueRO.position;
            ref var prevOffsets = ref m_type.ValueRO.asset.Value.rotationOffsets[m_localTransform.ValueRO.orientation].offsets;
            ref var newOffsets = ref m_type.ValueRO.asset.Value.rotationOffsets[newRotation].offsets;

            for (int i = 0; i < prevOffsets.Length; i++)
            {
                var newPosition = position + prevOffsets[i] - newOffsets[i];
                if (TrySetPositionAndRotation(newPosition, newRotation, newMatrix, collider))
                    return true;
            }

            return false;
        }
    }
}
