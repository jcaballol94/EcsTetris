using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class GridAuthoring : MonoBehaviour
    {
        public float BlockSize = 1f;
        public Vector2Int Size = new Vector2Int(10, 40);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var height = Size.y * BlockSize;
            for (int i = 0; i <= Size.x; i++)
            {
                var start = transform.position + transform.right * i * BlockSize;
                Gizmos.DrawLine(start, start + Vector3.up * height);
            }
            var width = Size.x * BlockSize;
            for (int i = 0; i <= Size.y; i++)
            {
                var start = transform.position + transform.up * i * BlockSize;
                Gizmos.DrawLine(start, start + Vector3.right * width);
            }
        }
    }

    public class GridBaking : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            AddComponent(new GridToWorldData { blockSize = authoring.BlockSize });
            AddComponent(new GridSize { value = new int2(authoring.Size.x, authoring.Size.y) });
        }
    }
}
