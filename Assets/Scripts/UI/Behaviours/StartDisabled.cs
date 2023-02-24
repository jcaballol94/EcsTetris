using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class StartDisabled : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.PostProcessScene]
        public static void OnPostprocessScene()
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                var disabledComponents = root.GetComponentsInChildren<StartDisabled>();
                foreach (var comp in disabledComponents)
                {
                    comp.gameObject.SetActive(false);
                }
            }
        }
#endif
    }
}
