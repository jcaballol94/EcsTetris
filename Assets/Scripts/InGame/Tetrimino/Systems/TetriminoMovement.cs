using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct TetriminoMovementSystem : ISystem
    {
        private GridCollisions.Lookup m_colliderLookup;

        public partial struct TetriminoMovementJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public ComponentLookup<InputValues> inputLookup;
            [ReadOnly] public GridCollisions.Lookup colliderLookup;

            private void Execute(Entity entity, TetriminoMovement movement, in PlayerCleanupRef player, in GridRef grid)
            {
                var input = inputLookup[player.value];
                var collider = colliderLookup[grid.value];

                if (input.move != 0)
                {
                    var moved = movement.TryMove(new int2(input.move, 0), collider);
                    ecb.AddComponent(entity, new AudioRequest { effect = moved ? GameAudioManager.EFFECTS.Move : GameAudioManager.EFFECTS.Hit });

                }

                if (input.rotate != 0)
                {
                    var rotated = movement.TryRotate(input.rotate, collider);
                    ecb.AddComponent(entity, new AudioRequest { effect = rotated ? GameAudioManager.EFFECTS.Rotate : GameAudioManager.EFFECTS.Hit });

                }
            }
        }
        public void OnCreate(ref SystemState state)
        {
            m_colliderLookup = new GridCollisions.Lookup(ref state, true);

            state.RequireForUpdate<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>(out var systemEcb)) return;
            var ecb = systemEcb.CreateCommandBuffer(state.WorldUnmanaged);

            var inputLookup = SystemAPI.GetComponentLookup<InputValues>(true);
            m_colliderLookup.Update(ref state);

            new TetriminoMovementJob
            {
                ecb = ecb,
                colliderLookup = m_colliderLookup,
                inputLookup = inputLookup
            }.Schedule();
        }
    }
}