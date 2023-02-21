using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct GridCollisions : IAspect
    {
        private readonly RefRO<GridBounds> m_bounds;
        [ReadOnly] private readonly DynamicBuffer<GridCellData> m_cellData;

        public bool IsPositionValid(int2 position)
        {
            return IsPositionInBounds(position) && IsPositionAvailable(position);
        }

        private bool IsPositionInBounds(int2 position)
        {
            var bounds = m_bounds.ValueRO.size;

            return position.x >= 0
                && position.x < bounds.x
                && position.y >= 0
                && position.y < bounds.y;
        }

        private bool IsPositionAvailable(int2 position)
        {
            var idx = position.y * m_bounds.ValueRO.size.x + position.x;
            return m_cellData[idx].available;
        }
    }
}
