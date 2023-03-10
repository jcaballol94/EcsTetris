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

        [MenuItem("Assets/Create/Scripts/ECS/Buffer", priority = 0)]
        public static void CreateEcsBuffer()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/BufferTemplate.cs", "NewBuffer.cs");
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

        [MenuItem("Assets/Create/Scripts/ECS/ISystem", priority = 0)]
        public static void CreateISystem()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/ISystemTemplate.cs", "NewSystem.cs");
        }

        [MenuItem("Assets/Create/Scripts/ECS/SystemGroup", priority = 0)]
        public static void CreateSystemGroup()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/SystemGroupTemplate.cs", "NewSystemGroup.cs");
        }

        [MenuItem("Assets/Create/Scripts/ECS/CommandBufferSystem", priority = 0)]
        public static void CreateCommandBufferSystem()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/CommandBufferSystemTemplate.cs", "NewCommandBufferSystem.cs");
        }

        [MenuItem("Assets/Create/Scripts/ECS/Event", priority = 0)]
        public static void CreateEvent()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Templates/EventTemplate.cs", "NewEvent.cs");
        }
    }
}
