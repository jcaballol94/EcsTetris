using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [AddComponentMenu("Tetris/InGame/Grid")]
    public class GridAuthoring : MonoBehaviour
    {
        public float gridScale = 1f;
    }

    public class GridAuthoringBaking : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var transform = GetComponent<Transform>();

            AddComponent(new GridTransformData
            {
                origin = transform.position,
                up = transform.up,
                right = transform.right,
                scale = authoring.gridScale
            });
        }
    }
}