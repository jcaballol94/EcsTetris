using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tetris
{
    public class LoadSceneButton : ButtonBehaviour
    {
        public int sceneIdxToLoad = 0;
        public LoadSceneMode mode = LoadSceneMode.Single;

        public override void OnClick()
        {
            SceneManager.LoadSceneAsync(sceneIdxToLoad, mode);
        }
    }
}
