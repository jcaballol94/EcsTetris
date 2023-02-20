using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct GridCollisions : IAspect
    {
        private readonly RefRO<GridBounds> m_bounds;

        public bool IsPositionValid(int2 position)
        {
            return IsPositionInBounds(position);
        }

        private bool IsPositionInBounds(int2 position)
        {
            var bounds = m_bounds.ValueRO.size;

            return position.x >= 0
                && position.x < bounds.x
                && position.y >= 0
                && position.y < bounds.y;
        }
    }
}
