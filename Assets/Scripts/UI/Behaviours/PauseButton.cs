using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris
{
    [RequireComponent(typeof(Button))]
    public class PauseButton : MonoBehaviour
    {
        private EntityArchetype m_archetype;
        private Button m_button;

        public void OnEnable()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            m_archetype = entityManager.CreateArchetype(typeof(TogglePauseTag));

            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(OnClick);
        }

        public void OnDisable()
        {
            m_button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.CreateEntity(m_archetype);
        }
    }
}
