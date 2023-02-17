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

            // Store the previous provider to be able to cleanup
            foreach (var (provider, entity) in SystemAPI.Query<InputProvider>().WithNone<PrevInputProvider>().WithEntityAccess())
            {
                ecb.AddComponent(entity, new PrevInputProvider { value = provider.value });
            }

            // If an entity only has the cleanup component, do the cleanup by unregistering it from the provider
            var listenerLookup = SystemAPI.GetBufferLookup<InputListener>();
            foreach (var (provider, entity) in SystemAPI.Query<PrevInputProvider>().WithNone<InputProvider>().WithEntityAccess())
            {
                var buffer = listenerLookup[provider.value];
                for (int i =0; i < buffer.Length; ++i)
                {
                    if (buffer[i].value == entity)
                    {
                        buffer.RemoveAtSwapBack(i);
                        break;
                    }
                }
                ecb.RemoveComponent<PrevInputProvider>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}