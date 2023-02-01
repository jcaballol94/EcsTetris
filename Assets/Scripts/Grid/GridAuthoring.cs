using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [AddComponentMenu("Tetris/Grid/Grid")]
    public class GridAuthoring : MonoBehaviour
    {
        public float BlockSize = 1f;
    }

    public class GridBaking : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var transform = GetComponent<Transform>();

            AddComponent(new GridToWorldData
            {
                blockSize = authoring.BlockSize,
                origin = transform.position,
                up = transform.up,
                right = transform.right,
            });
        }
    }
}