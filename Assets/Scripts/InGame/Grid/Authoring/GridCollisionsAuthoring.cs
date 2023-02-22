using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireComponent(typeof(GridSizeAuthoring))]
    public class GridCollisionsAuthoring : MonoBehaviour
    {
    }

    public class GridCollisionsAuthoringBaking : Baker<GridCollisionsAuthoring>
    {
        public override void Bake(GridCollisionsAuthoring authoring)
        {
            AddComponent<GridWithCollisionsTag>();
        }
    }
}