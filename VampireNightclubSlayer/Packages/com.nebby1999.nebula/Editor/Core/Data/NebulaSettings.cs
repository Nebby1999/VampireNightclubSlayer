using Nebula.Editor.CodeGenerators;
using System;
using UnityEditor;
using UnityEngine;
#if USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nebula.Editor
{
    [FilePath("ProjectSettings/NebulaSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class NebulaSettings : ScriptableSingleton<NebulaSettings>
    {
#if USE_INPUT_SYSTEM
        public InputActionGUIDData[] inputActionGuidDatas = Array.Empty<InputActionGUIDData>();
#endif
        public bool createLayerIndexStruct = true;
        public LayerIndexData layerIndexData;
        public bool createGameTagsStruct = true;
        public GameTagsData gameTagsData;


        internal void DoSave()
        {
            Save(false);
        }

        [MenuItem("Tools/Nebula/Generate Layer Index")]
        private static void MenuItem_GenerateLayerIndexStruct()
        {
            instance.GenerateLayerIndexStruct();
        }

        [MenuItem("Tools/Nebula/Generate Layer Index", true)]
        private static bool MenuItemValidate_GenerateLayerIndexStruct()
        {
            return instance.createLayerIndexStruct;
        }

        [MenuItem("Tools/Nebula/Generate Game Tags Struct")]
        private static void MenuItem_GenerateGameTagsStruct()
        {
            instance.GenerateGameTagsStruct();
        }

        [MenuItem("Tools/Nebula/Generate Game Tags Struct", true)]
        private static bool MenuItemValidate_GenerateGameTagsStruct()
        {
            return instance.createGameTagsStruct;
        }


#if USE_INPUT_SYSTEM
        [MenuItem("Tools/Nebula/Generate Input Action GUID Classes")]
        private static void MenuItem_GenerateInputActionGUIDClasses()
        {
            instance.GenerateInputActionGUIDClasses();
        }
#endif

#if USE_INPUT_SYSTEM
        [MenuItem("Tools/Nebula/Generate Input Action GUID Classes", true)]
        private static bool MenuItemValidate_GenerateInputActionGUIDClasses()
        {
            return instance.inputActionGuidDatas.Length != 0;
        }
#endif

        internal void GenerateLayerIndexStruct()
        {
            if (createLayerIndexStruct)
            {
                LayerIndexCodeGenerator.GenerateCode(layerIndexData);
            }
        }

#if USE_INPUT_SYSTEM
        internal void GenerateInputActionGUIDClasses()
        {
            for(int i = 0; i < inputActionGuidDatas.Length; i++)
            {
                InputActionGUIDCodeGenerator.GenerateCode(inputActionGuidDatas[i]);
            }
        }
#endif

        internal void GenerateGameTagsStruct()
        {
            if(createGameTagsStruct)
            {
                GameTagsCodeGenerator.GenerateCode(gameTagsData);
            }
        }

        [Serializable]
        public struct LayerIndexData
        {
            public bool is2D;
            public CommonMask[] commonMaskSelector;
            [FilePickerPath(defaultName = "LayerIndex", extension = "cs", title = "Location for generated C# file")]
            public string filePath;
            public string nameSpace;

            [Serializable]
            public struct CommonMask
            {
                public string comment;
                public string maskName;
                public LayerMask layerMask;
            }
        }

        [Serializable]
        public struct GameTagsData
        {
            [FilePickerPath(defaultName = "GameTags", extension = "cs", title = "Location for generated C# file")]
            public string filePath;
            public string nameSpace;
        }

#if USE_INPUT_SYSTEM
        [Serializable]
        public struct InputActionGUIDData
        {
            public InputActionAsset inputActionAsset;
            [FilePickerPath(defaultName = "InputActionGUIDS", extension = "cs", title = "Location for generated C# file")]
            public string filePath;
            public string nameSpace;
        }
#endif
    }
}