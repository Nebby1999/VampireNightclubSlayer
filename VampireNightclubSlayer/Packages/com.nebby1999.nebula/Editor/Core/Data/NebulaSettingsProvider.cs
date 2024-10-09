using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebula.Editor
{
    public sealed class NebulaSettingsProvider : SettingsProvider
    {
        private NebulaSettings _settings;
        private SerializedObject _serializedObject;

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var keywords = new[] { "Nebula" };
            var settings = NebulaSettings.instance;
            settings.hideFlags = UnityEngine.HideFlags.DontSave | UnityEngine.HideFlags.HideInHierarchy;
            settings.DoSave();
            return new NebulaSettingsProvider("Project/Nebula", SettingsScope.Project, keywords)
            {
                _settings = settings,
                _serializedObject = new SerializedObject(settings)
            };
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var templateRoot = VisualElementTemplateFinder.GetTemplateInstance("NebulaSettingsElement", rootElement);
            rootElement.Bind(_serializedObject);
            InitializeElement(templateRoot);
        }

        private void InitializeElement(VisualElement templateRoot)
        {
            var enableLayerIndexGenerationToggle = templateRoot.Q<Toggle>("EnableLayerIndexGeneration");
            var regenerateLayerIndexButton = templateRoot.Q<Button>("RegenerateLayerIndexStructButton");
            var layerIndexContents = templateRoot.Q<VisualElement>("LayerIndexContents");
            enableLayerIndexGenerationToggle.RegisterValueChangedCallback((evt) =>
            {
                var val = evt.newValue;
                regenerateLayerIndexButton.SetEnabled(val);
                layerIndexContents.SetEnabled(val);
            });
            regenerateLayerIndexButton.SetEnabled(enableLayerIndexGenerationToggle.value);
            regenerateLayerIndexButton.clicked += RegenerateLayerIndexButton_clicked;

            layerIndexContents.SetEnabled(enableLayerIndexGenerationToggle.value);

            var enableGameTagsGenerationToggle = templateRoot.Q<Toggle>("EnableGameTagsGeneration");
            var regenerateGameTagsButton = templateRoot.Q<Button>("RegenerateGameTagsStructButton");
            var gameTagsContents = templateRoot.Q<VisualElement>("GameTagsContents");
            enableGameTagsGenerationToggle.RegisterValueChangedCallback((evt) =>
            {
                var val = evt.newValue;
                regenerateGameTagsButton.SetEnabled(val);
                gameTagsContents.SetEnabled(val);
            });
            regenerateGameTagsButton.SetEnabled(enableLayerIndexGenerationToggle.value);
            regenerateGameTagsButton.clicked += RegenerateGameTagsButton_clicked;
            var regenerateInputGUIDClassButton = templateRoot.Q<Button>("RegenerateInputGUIDClassButton");
#if USE_INPUT_SYSTEM
            regenerateInputGUIDClassButton.clicked += RegenerateInputGUIDClassButton_clicked;
#else
            regenerateInputGUIDClassButton.SetDisplay(false);
#endif
        }

        private void RegenerateGameTagsButton_clicked()
        {
            _settings.GenerateGameTagsStruct();
        }

#if USE_INPUT_SYSTEM
        private void RegenerateInputGUIDClassButton_clicked()
        {
            _settings.GenerateInputActionGUIDClasses();
        }
#endif

        private void RegenerateLayerIndexButton_clicked()
        {
            _settings.GenerateLayerIndexStruct();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            Save();
        }

        private void Save()
        {
            _serializedObject?.ApplyModifiedProperties();
            if (_settings)
                _settings.DoSave();
        }

        public NebulaSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }
    }
}