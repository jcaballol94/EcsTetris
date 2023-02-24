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
        private readonly DynamicBuffer<GridCellData> m_cellData;

        public int PositionToIdx(int2 position)
        {
            return position.y * m_bounds.ValueRO.size.x + position.x;
        }

        public bool IsPositionValid(int2 position)
        {
            return IsPositionInBounds(position) && IsPositionAvailable(position);
        }

        public void TakePosition(int2 position)
        {
            var idx = PositionToIdx(position);
            m_cellData.ElementAt(idx).available = false;
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
            var idx = PositionToIdx(position);
            return m_cellData[idx].available;
        }
    }
}
