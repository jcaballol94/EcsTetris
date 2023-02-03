using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct GridQueryAspect : IAspect
    {
        public readonly Entity Self;

        readonly RefRO<GridBounds> m_bounds;

        public bool IsPositionAvailable(int2 position)
        {
            if (!IsPositionInBounds(position))
                return false;

            return true;
        }

        private bool IsPositionInBounds(int2 position)
        {
            var bounds = m_bounds.ValueRO.value;

            return position.x >= 0 && position.x < bounds.x
                && position.y >= 0 && position.y < bounds.y;
        }
    }
}
