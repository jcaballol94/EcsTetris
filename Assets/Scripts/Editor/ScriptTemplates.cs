using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tetris
{
    public static class ScriptTemplates
    {
        [MenuItem("Assets/Create/Scripts/ECS/Component", priority = 0)]
        public static void CreateEcsComponent()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/ComponentTemplate.cs", "NewComponent.cs");
        }
    }
}
