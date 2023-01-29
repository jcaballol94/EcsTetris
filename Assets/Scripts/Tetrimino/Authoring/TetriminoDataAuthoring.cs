using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class TetriminoDataAuthoring : MonoBehaviour
    {
        public Vector2Int spawnPos = new Vector2Int(4, 9);
        public GameObject blockPrefab;
        public TetriminoDefinition[] availableTetriminos;
    }

    public class TetriminoDataBaking : Baker<TetriminoDataAuthoring>
    {
        public override void Bake(TetriminoDataAuthoring authoring)
        {
            // Add the prefab
            if (authoring.blockPrefab)
                AddComponent(new BlockPrefab 
                { value = GetEntity(authoring.blockPrefab), 
                    spawnPos = new int2(authoring.spawnPos.x, authoring.spawnPos.y) 
                });

            // Create the entities for all the definitions
            foreach (var tetrimino in authoring.availableTetriminos)
            {
                if (!tetrimino)
                    continue;

                var entity = CreateAdditionalEntity(TransformUsageFlags.None);

                AddComponent(entity, new TetriminoData { color = new float4(tetrimino.color.r, tetrimino.color.g, tetrimino.color.b, tetrimino.color.a) });

                var blocks = AddBuffer<TetriminoBlockDefinition>(entity);
                foreach (var block in tetrimino.blocks)
                {
                    blocks.Add(new TetriminoBlockDefinition { Value = new int2(block.x, block.y) });
                }

                var offsets = AddBuffer<TetriminoOffsetDefinition>(entity);
                foreach (var offset in tetrimino.rotationOffsets)
                {
                    offsets.Add(new TetriminoOffsetDefinition { value = new int2(offset.x, offset.y) });
                }
            }
        }
    }
}