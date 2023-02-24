using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class RegisterGameObject : MonoBehaviour
    {
        public GameObject target;
        public GameObjectVariable variable;

        private void Awake()
        {
            if (target && variable)
                variable.value = target;
        }
    }
}