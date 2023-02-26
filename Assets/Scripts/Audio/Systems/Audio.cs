using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct AudioSystem : ISystem
    {
        public partial struct AudioJob : IJobEntity
        {
            [BurstCompile]
            private void Execute()
            {
            }
        }

        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var audio = GameAudioManager.Instance;

            foreach (var request in SystemAPI.Query<RefRO<AudioRequest>>())
            {
                Debug.Log("Requested: " + request.ValueRO.effect);
                audio.PlayEffect(request.ValueRO.effect);
            }

            state.EntityManager.RemoveComponent<AudioRequest>(SystemAPI.QueryBuilder().WithAll<AudioRequest>().Build());
        }
    }
}