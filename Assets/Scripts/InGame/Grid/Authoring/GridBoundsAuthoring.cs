using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [AddComponentMenu("Tetris/Grid Size")]
    [RequireComponent(typeof(GridAuthoring))]
    public class GridSizeAuthoring : MonoBehaviour
    {
        public Vector2Int bounds = new Vector2Int(10, 40);

        private GridAuthoring m_grid;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            if (!m_grid) m_grid = GetComponent<GridAuthoring>();

            var height = transform.up * m_grid.gridScale * bounds.y;
            for (int i = 0; i <= bounds.x; i++)
            {
                var point = transform.position + transform.right * m_grid.gridScale * i;
                Gizmos.DrawLine(point, point + height);
            }

            var width = transform.right * m_grid.gridScale * bounds.x;
            for (int i = 0; i <= bounds.y; i++)
            {
                var point = transform.position + transform.up * m_grid.gridScale * i;
                Gizmos.DrawLine(point, point + width);
            }
        }
    }

    public class GridSizeBaking : Baker<GridSizeAuthoring>
    {
        public override void Bake(GridSizeAuthoring authoring)
        {
            AddComponent(new GridBounds { size = new int2(authoring.bounds.x, authoring.bounds.y) });
        }
    }
}