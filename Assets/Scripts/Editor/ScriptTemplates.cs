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

        [MenuItem("Assets/Create/Scripts/ECS/Aspect", priority = 0)]
        public static void CreateAspect()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/AspectTemplate.cs", "NewAspect.cs");
        }

        [MenuItem("Assets/Create/Scripts/ECS/Authoring", priority = 0)]
        public static void CreateAuthoring()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/AuthoringTemplate.cs", "NewAuthoring.cs");
        }
    }
}
