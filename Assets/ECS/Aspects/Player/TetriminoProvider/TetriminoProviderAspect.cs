using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct TetriminoProviderAspect : IAspect
    {
        public readonly Entity Self;

        private readonly RefRO<AvailableTetriminos> m_availableTetriminos;

        public static void SetupEntity(ref EntityCommandBuffer ecb, Entity entity, in BlobAssetReference<AvailableTetrimnosBlob> data)
        {
            ecb.AddComponent(entity, new AvailableTetriminos { value = data });
        }
    }
}
