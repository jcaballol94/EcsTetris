using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(DetectLinesSystem))]
    [UpdateBefore(typeof(RemoveLinesSystem))]
    public partial struct UpdateScoresSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var score in SystemAPI.Query<RefRO<CurrentScore>>().WithChangeFilter<CurrentScore>())
            {
                if (!GameObjectReferences.Instance.TryGetObject("ScoresPanel", out var scoresGO)) break;
                var scoresPanel = scoresGO.GetComponent<ScoresPanel>();

                scoresPanel.UpdateValues(score.ValueRO);
            }
        }
    }
}