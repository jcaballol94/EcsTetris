using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class GameModeAuthoring : MonoBehaviour
    {
        public TetriminoDefinition[] availableTetriminos;
        public GridAuthoring[] playerGrids;
        public Vector2Int spawnPosition;
        public GameObject blockPrefab;
        public float moveRepeatDelay = 0.5f;
        public float moveRepeatRatio = 0.2f;
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            AddComponent<ActiveGameModeTag>();

            if (authoring.blockPrefab)
                AddComponent(new BlockPrefab { value = GetEntity(authoring.blockPrefab) });

            AddSharedComponent(new GameSettings 
            { 
                spawnPosition = new int2(authoring.spawnPosition.x, authoring.spawnPosition.y),
                moveRepeatDelay = authoring.moveRepeatDelay,
                moveRepeatRatio = authoring.moveRepeatRatio
            });

            if (authoring.playerGrids != null && authoring.playerGrids.Length > 0)
            {
                var playersBuffer = AddBuffer<PlayerDefinitionBuffer>();
                foreach (var grid in authoring.playerGrids) 
                {
                    if (grid)
                        playersBuffer.Add(new PlayerDefinitionBuffer { grid = GetEntity(grid.gameObject)});
                }
            }

            if (authoring.availableTetriminos != null && authoring.availableTetriminos.Length > 0)
            {
                var tetriminosBuffer = AddBuffer<AvailableTetriminoBuffer>();
                foreach (var tetrimino in authoring.availableTetriminos)
                {
                    var blob = tetrimino.CreateBlobAsset();
                    AddBlobAsset(ref blob, out var hash);

                    tetriminosBuffer.Add(new AvailableTetriminoBuffer { asset = blob });
                }
            }
        }
    }
}