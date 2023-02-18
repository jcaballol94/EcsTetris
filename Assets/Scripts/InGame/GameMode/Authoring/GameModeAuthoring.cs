using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [AddComponentMenu("Tetris/InGame/Game Mode")]
    public class GameModeAuthoring : MonoBehaviour
    {
        public GridAuthoring grid;
        public GameObject blockPrefab;
        public Vector2Int spawnPosition;
        public TetriminoDefinition tetrimino;
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            if (authoring.grid)
                AddSharedComponent(new GridRef { value = GetEntity(authoring.grid.gameObject) });

            if (authoring.blockPrefab)
                AddComponent(new GameSkin { blockPrefab = GetEntity(authoring.blockPrefab) });

            if (authoring.tetrimino)
            {
                DependsOn(authoring.tetrimino);
                var blob = authoring.tetrimino.CreateBlobAsset();
                AddBlobAsset(ref blob, out var hash);

                AddComponent(new TetriminoData { asset = blob });
            }

            AddComponent(new GameData
            {
                spawnPosition = new int2(authoring.spawnPosition.x, authoring.spawnPosition.y)
            });
        }
    }
}