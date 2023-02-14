using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InputSystemGroup), OrderFirst = true)]
    public partial struct SetupInputComponentsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Link all the providers with all the 
            foreach (var (provider, entity) in SystemAPI.Query<InputProvider>().WithNone<InputValues>().WithEntityAccess())
            {
                ecb.AddComponent<InputValues>(entity);
                var buffer = ecb.AddBuffer<InputListener>(provider.value);
                buffer.Add(new InputListener { value = entity });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}