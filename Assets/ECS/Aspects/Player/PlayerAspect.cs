using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public readonly partial struct PlayerAspect : IAspect
    {
        public readonly Entity Self;

        private readonly TetriminoProviderAspect m_tetriminoProvider;

        public static void SeetupEntity(Entity entity, ref EntityCommandBuffer ecb, 
            in BlobAssetReference<AvailableTetrimnosBlob> availableTetriminos)
        {
            ecb.AddComponent<PlayerTag>(entity);
            TetriminoProviderAspect.SetupEntity(ref ecb, entity, availableTetriminos);
        }
    }
}
