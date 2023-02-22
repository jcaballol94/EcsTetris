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
        public TetriminoDefinition[] tetriminos;

        [Header("Input")]
        public float moveRepeatDelay = 0.3f;
        public float moveRepeatPeriod = 0.2f;

        [Header("Drop")]
        public float fallSpeed = 1f;
        public float fastFallMultiplier = 5f;
        public int dropLength = 40;
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            if (authoring.grid)
            {
                var gridEntity = GetEntity(authoring.grid.gameObject);
                AddComponent(new PlayerData { grid = gridEntity });
            }

            if (authoring.blockPrefab)
                AddComponent(new GameSkin { blockPrefab = GetEntity(authoring.blockPrefab) });

            if (authoring.tetriminos != null && authoring.tetriminos.Length > 0)
            {
                var buffer = AddBuffer<AvailableTetriminos>();
                foreach (var tetrimino in authoring.tetriminos)
                {
                    if (!tetrimino) continue;

                    DependsOn(tetrimino);
                    var blob = tetrimino.CreateBlobAsset();
                    AddBlobAsset(ref blob, out var hash);

                    buffer.Add(new AvailableTetriminos { asset = blob });
                }
            }

            AddComponent(new GameData
            {
                spawnPosition = new int2(authoring.spawnPosition.x, authoring.spawnPosition.y),
                moveRepeatDelay = authoring.moveRepeatDelay,
                moveRepeatPeriod = authoring.moveRepeatPeriod,
                fallSpeed = authoring.fallSpeed,
                fastFallMultiplier = authoring.fastFallMultiplier,
                dropLength = authoring.dropLength
            });
        }
    }
}