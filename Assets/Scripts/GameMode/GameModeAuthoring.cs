using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [AddComponentMenu("Tetris/Game Mode")]
    public class GameModeAuthoring : MonoBehaviour
    {
        public GridAuthoring mainGrid;
        public Vector2Int spawnPoint = new Vector2Int(4,10);
        public GameObject blockPrefab;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            if (!mainGrid)
                return;

            Gizmos.DrawSphere(mainGrid.transform.TransformPoint(new Vector3(spawnPoint.x + 0.5f, spawnPoint.y + 0.5f) * mainGrid.BlockSize), 0.1f);
        }
    }

    public class GameModeBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            if (!authoring.mainGrid || !authoring.blockPrefab)
                return;

            AddComponent(new TetrisGameMode
            {
                blockPrefab = GetEntity(authoring.blockPrefab),
                mainGrid = GetEntity(authoring.mainGrid),
                spawnPosition = new int2(authoring.spawnPoint.x, authoring.spawnPoint.y)
            });
        }
    }
}