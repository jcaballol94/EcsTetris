using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class RegisterObject : MonoBehaviour
    {
        public string key = "GameObject";
        public GameObject target;

        private void Start()
        {
            GameObjectReferences.Instance.RegisterObject(key, target);
        }

        private void OnDestroy()
        {
            GameObjectReferences.Instance.UnregisterObject(key);
        }
    }
}
