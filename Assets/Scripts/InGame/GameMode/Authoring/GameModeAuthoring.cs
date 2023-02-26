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
        public GridAuthoring mainGrid;
        public GridAuthoring nextGrid;
        public GridAuthoring holdGrid;
        public GameObject blockPrefab;
        public TetriminoDefinition[] tetriminos;

        [Header("Input")]
        public float moveRepeatDelay = 0.3f;
        public float moveRepeatPeriod = 0.2f;

        [Header("Drop")]
        public float[] fallSpeeds;
        public float fastFallMultiplier = 5f;
        public int dropLength = 40;
        public float removeLineDuration = 0.5f;

        [Header("Spawning")]
        public Vector2Int spawnPosition;
        public float baseSpawnDelay = 10f / 60f;
        public float spawnDelayDelta = 2f / 60f;

        public struct SpeedsBlob
        {
            public BlobArray<float> values;
        }
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            if (authoring.mainGrid && authoring.nextGrid && authoring.holdGrid)
            {
                var gridEntity = GetEntity(authoring.mainGrid.gameObject);
                var nextGridEntity = GetEntity(authoring.nextGrid.gameObject);
                var holdGridEntity = GetEntity(authoring.holdGrid.gameObject);
                AddComponent(new PlayerData 
                { 
                    mainGrid = gridEntity,
                    nextGrid = nextGridEntity,
                    holdGrid = holdGridEntity
                });
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

            BlobAssetReference<GameModeAuthoring.SpeedsBlob> speedsRef = default;
            if (authoring.fallSpeeds != null && authoring.fallSpeeds.Length > 0)
            {
                var builder = new BlobBuilder(Unity.Collections.Allocator.Temp);
                ref var root = ref builder.ConstructRoot<GameModeAuthoring.SpeedsBlob>();
                var arrayBuilder = builder.Allocate(ref root.values, authoring.fallSpeeds.Length);
                
                for (int i = 0; i < authoring.fallSpeeds.Length; ++i)
                {
                    arrayBuilder[i] = authoring.fallSpeeds[i];
                }

                speedsRef = builder.CreateBlobAssetReference<GameModeAuthoring.SpeedsBlob>(Unity.Collections.Allocator.Persistent);
                builder.Dispose();

                AddBlobAsset(ref speedsRef, out var hash);
            }

            AddComponent(new GameData
            {
                spawnPosition = new int2(authoring.spawnPosition.x, authoring.spawnPosition.y),
                moveRepeatDelay = authoring.moveRepeatDelay,
                moveRepeatPeriod = authoring.moveRepeatPeriod,
                fallSpeed = speedsRef,
                fastFallMultiplier = authoring.fastFallMultiplier,
                dropLength = authoring.dropLength,
                baseSpawnDelay = authoring.baseSpawnDelay,
                spawnDelayDelta = authoring.spawnDelayDelta,
                removeLineDuration = authoring.removeLineDuration
            });

            AddComponent(new TimeScale { value = 1f });
            AddComponent<ScaledDeltaTime>();
        }
    }
}