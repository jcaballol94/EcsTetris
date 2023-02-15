using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct GridCollider : IAspect
    {
        private readonly RefRO<GridSize> m_bounds;

        public bool IsPositionInBounds(int2 position)
        {
            return position.x >= 0
                && position.x < m_bounds.ValueRO.value.x
                && position.y >= 0
                && position.y < m_bounds.ValueRO.value.y;
        }

        public bool IsPositionValid(int2 position)
        {
            return IsPositionInBounds(position);
        }
    }
}
