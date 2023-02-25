using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class GameObjectReferences : Singleton<GameObjectReferences>
    {
        private Dictionary<string, GameObject> m_objects = new Dictionary<string, GameObject>();

        public void RegisterObject(string key, GameObject go)
        {
            m_objects[key] = go;
        }

        public void UnregisterObject(string key)
        {
            m_objects.Remove(key);
        }

        public bool TryGetObject(string key, out GameObject go)
        {
            return m_objects.TryGetValue(key, out go);
        }
    }
}