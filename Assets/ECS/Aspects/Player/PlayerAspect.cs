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

        private readonly RefRO<PlayerTag> m_tag;

        public static void SetupPlayer(Entity entity, EntityCommandBuffer buffer)
        {
            buffer.AddComponent<PlayerTag>(entity);
        }
    }
}
