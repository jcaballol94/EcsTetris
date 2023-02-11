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
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            AddComponent<ActiveGameModeTag>();

            // Add the players
            var playersBuffer = AddBuffer<PlayerDefinitionBuffer>();
            playersBuffer.Add(new PlayerDefinitionBuffer());

            if (authoring.availableTetriminos != null && authoring.availableTetriminos.Length > 0)
            {
                // Add the available blobs
                var tetriminosBuffer = AddBuffer<AvailableTetriminoBuffer>();
                foreach (var tetrimino in authoring.availableTetriminos)
                {
                    // Register the tetrimino as a dependency to update the blob assets
                    DependsOn(tetrimino);
                    // Create the asset
                    var tetriminoBlob = tetrimino.CreateBlobAsset();
                    AddBlobAsset(ref tetriminoBlob, out var hash);
                    tetriminosBuffer.Add(new AvailableTetriminoBuffer { blob = tetriminoBlob });
                }
            }
        }
    }
}