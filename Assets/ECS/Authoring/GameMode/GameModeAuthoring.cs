using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class GameModeAuthoring : MonoBehaviour
    {
        public int numPlayers = 1;
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            AddComponent<ActiveGameModeTag>();
            var buffer = AddBuffer<PlayerDefinitionBuffer>();
            for (int i = 0; i < authoring.numPlayers; ++i)
            {
                buffer.Add(new PlayerDefinitionBuffer());
            }
        }
    }
}