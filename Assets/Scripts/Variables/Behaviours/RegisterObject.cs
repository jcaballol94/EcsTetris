using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class RegisterObject : MonoBehaviour
    {
        public string key;
        public GameObject target;

        private void Reset()
        {
            key = name;
            target = gameObject;
        }

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
