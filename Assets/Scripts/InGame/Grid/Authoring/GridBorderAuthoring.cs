using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireComponent(typeof(GridSizeAuthoring))]
    public class GridBorderAuthoring : MonoBehaviour
    {
        public Color color;
    }

    public class GridBorderAuthoringBaking : Baker<GridBorderAuthoring>
    {
        public override void Bake(GridBorderAuthoring authoring)
        {
            AddComponent(new GridBorder { color = new float4(authoring.color.r, authoring.color.g, authoring.color.b, authoring.color.a) });
        }
    }
}