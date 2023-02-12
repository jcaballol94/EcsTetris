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
        public Vector2Int spawnPosition;
        public GameObject blockPrefab;
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            AddComponent<ActiveGameModeTag>();

            if (authoring.blockPrefab)
                AddComponent(new BlockPrefab { value = GetEntity(authoring.blockPrefab) });

            AddComponent(new SpawnPosition { value = new int2(authoring.spawnPosition.x, authoring.spawnPosition.y) });

            var playersBuffer = AddBuffer<PlayerDefinitionBuffer>();
            playersBuffer.Add(new PlayerDefinitionBuffer());

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