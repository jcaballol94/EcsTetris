using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct PlayerSpawner : IAspect
    {
        private readonly DynamicBuffer<PlayerDefinitionBuffer> m_definitions;
        private readonly DynamicBuffer<AvailableTetriminoBuffer> m_tetriminos;
        private readonly RefRO<SpawnPosition> m_spawnPos;
        private readonly RefRO<BlockPrefab> m_blockPrefab;

        public int PlayersToCreate => m_definitions.Length;

        public void SetupEntity(ref EntityCommandBuffer ecb, Entity playerEntity, int playerIdx)
        {
            ecb.AddComponent<PlayerTag>(playerEntity);
            ecb.AddComponent(playerEntity, m_blockPrefab.ValueRO);
            ecb.AddComponent(playerEntity, m_spawnPos.ValueRO);
            ecb.AddComponent(playerEntity, new GridRef { value = m_definitions[playerIdx].grid });

            var tetriminoesBuffer = ecb.AddBuffer<AvailableTetriminoBuffer>(playerEntity);
            foreach (var tetrimino in m_tetriminos)
            {
                tetriminoesBuffer.Add(tetrimino);
            }
        }

        public Entity SpawnEntity(ref EntityCommandBuffer ecb, int playerIdx)
        {
            var playerEntity = ecb.CreateEntity();
            ecb.SetName(playerEntity, "Player");

            SetupEntity(ref ecb, playerEntity, playerIdx);

            return playerEntity;
        }
    }
}
