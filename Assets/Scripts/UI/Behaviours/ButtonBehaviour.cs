using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBehaviour : MonoBehaviour
    {
        protected Button m_button;

        protected virtual void Awake()
        {
            m_button = GetComponent<Button>();
        }

        protected virtual void OnEnable()
        {
            m_button.onClick.AddListener(OnClick);
        }

        protected virtual void OnDisable()
        {
            m_button.onClick.RemoveListener(OnClick);
        }

        public abstract void OnClick();
    }
}
