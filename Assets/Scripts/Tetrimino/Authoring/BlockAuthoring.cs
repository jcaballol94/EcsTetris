using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class BlockAuthoring : MonoBehaviour
    {
        public Vector2Int pos = new Vector2Int(3, 3);
    }

    public class BlockAuthoringBaking : Baker<BlockAuthoring>
    {
        public override void Bake(BlockAuthoring authoring)
        {
            AddComponent(new PositionInGrid { value = new int2(authoring.pos.x, authoring.pos.y) });
        }
    }
}