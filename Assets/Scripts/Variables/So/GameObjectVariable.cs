using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    [CreateAssetMenu(fileName = "GameObject", menuName = "Variables/Game Object", order = 0)]
    public class GameObjectVariable : ScriptableObject
    {
        public GameObject value;
    }
}
