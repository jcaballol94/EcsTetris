using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris
{
    public class PauseButton : ButtonBehaviour
    {
        private EntityArchetype m_archetype;

        protected override void OnEnable()
        {
            base.OnEnable();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            m_archetype = entityManager.CreateArchetype(typeof(TogglePauseTag));
        }

        public override void OnClick()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.CreateEntity(m_archetype);
        }
    }
}
