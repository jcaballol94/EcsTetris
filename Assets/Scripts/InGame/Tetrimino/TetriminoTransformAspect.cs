using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct TetriminoTransformAspect : IAspect
    {
        private readonly RefRO<TetriminoPosition> m_position;
        [Optional] private readonly RefRO<TetriminoOrientationMatrix> m_matrix;

        public int2 TransformPoint(int2 point)
        {
            int2x2 matrix;
            if (m_matrix.IsValid)
                matrix = m_matrix.ValueRO.value;
            else
                matrix = TetriminoOrientationMatrix.GetMatrixForOrientation(m_position.ValueRO.orientation);

            return m_position.ValueRO.position + math.mul(point, matrix);
        }
    }
}
