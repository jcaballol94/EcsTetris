using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial struct ReadInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Initialize the input
            foreach (var (values, entity) in SystemAPI
                .Query<RefRO<InputValues>>()
                .WithNone<TetrisInputReader>()
                .WithEntityAccess())
            {
                var input = new TetrisInputReader();
                input.Initialize();

                ecb.AddComponent(entity, input);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // Read the values
            if (!SystemAPI.TryGetSingleton(out GameData gameData)) return;
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (reader, values) in SystemAPI
                .Query<TetrisInputReader, RefRW<InputValues>>())
            {
                reader.UpdateValues(ref values.ValueRW, gameData, deltaTime);
            }
        }
    }
}