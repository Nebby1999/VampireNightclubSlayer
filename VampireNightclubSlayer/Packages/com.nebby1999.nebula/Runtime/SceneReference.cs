#if !USE_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nebula
{
    [Serializable]
    public class SceneReference : IEqualityComparer<SceneReference>, IEquatable<SceneReference>
    {
        public static SceneReference @null => new SceneReference(null);
        public string sceneName => _sceneName;
        [SerializeField] private string _sceneName;
        public string scenePath => _scenePath;
        [SerializeField] private string _scenePath;
        public int sceneBuildIndex => _sceneBuildIndex;
        [SerializeField] private int _sceneBuildIndex = -1;

        public Scene GetScene()
        {
            var sceneThatMayOrMayNotBeValid = SceneManager.GetSceneByBuildIndex(_sceneBuildIndex);
            if (sceneThatMayOrMayNotBeValid.IsValid())
            {
                return sceneThatMayOrMayNotBeValid;
            }
            Debug.LogError($"SceneReference {this} represents an invalid scene.");
            return sceneThatMayOrMayNotBeValid;
        }

        public Scene LoadScene() => LoadScene(LoadSceneMode.Single);
        public Scene LoadScene(LoadSceneMode loadSceneMode)
        {
            return SceneManager.LoadScene(_sceneName, new LoadSceneParameters(loadSceneMode));
        }

        public AsyncOperation LoadSceneAsync() => LoadSceneAsync(LoadSceneMode.Single);
        public AsyncOperation LoadSceneAsync(LoadSceneMode loadSceneMode)
        {
            return SceneManager.LoadSceneAsync(_sceneName, loadSceneMode);
        }

        public override string ToString()
        {
            return $"{_sceneName} ({_scenePath})({_sceneBuildIndex})";
        }

        public bool Equals(SceneReference x, SceneReference y)
        {
            if (x.sceneBuildIndex == -1 || y.sceneBuildIndex == -1)
            {
                return string.Equals(x.sceneName, y.sceneName, StringComparison.Ordinal);
            }
            return x.sceneBuildIndex == y.sceneBuildIndex;
        }

        public int GetHashCode(SceneReference obj)
        {
            return _scenePath.GetHashCode();
        }

        public bool Equals(SceneReference other)
        {
            if (sceneBuildIndex == -1 || other.sceneBuildIndex == -1)
            {
                return string.Equals(sceneName, other.sceneName, StringComparison.Ordinal);
            }
            return sceneBuildIndex == other.sceneBuildIndex;
        }

        public SceneReference()
        {

        }

        public SceneReference(string sceneName)
        {
            _sceneName = sceneName;
        }
    }
}
#endif