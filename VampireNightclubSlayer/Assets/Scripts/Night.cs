using Nebula;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VampireSlayer
{
    //Manages a single night, stores stuff like the amount of lives the player has, the amount of people, etc.
    public class Night : MonoBehaviour
    {
        public static GameObject nightPrefab;

        public Xoroshiro128Plus nightRng { get; private set; }

        public int peopleAmount => _peopleAmount;
        [SerializeField]
        private int _peopleAmount;

        public int lives => _lives;
        [SerializeField]
        private int _lives;


        [AsyncAssetLoad]
        private static IEnumerator LoadAsync()
        {
            var request = Addressables.LoadAssetAsync<GameObject>("GameAssets/Night.prefab");
            while (!request.IsDone)
                yield return null;

            nightPrefab = request.Result;
            yield break;
        }

        private void StartNightInstance()
        {

        }
    }
}
