using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    [RequireComponent(typeof(UnityEngine.UI.Selectable))]
    public class StartSelected : MonoBehaviour
    {
        private void OnEnable()
        {
            var selectable = GetComponent<UnityEngine.UI.Selectable>();
            selectable.Select();
        }
    }
}
