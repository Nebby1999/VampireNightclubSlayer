using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nebula;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace VampireSlayer
{
    public class GameManager : MonoBehaviour
    {
        [Header("Spline Settings")]
        public UnityEngine.Splines.SplineContainer spawningSpline;

        [Header("Game Metadata")]
        public Night night;
        public int currentNight = -1;
        public ulong totalScore = 0;
        public List<ulong> scorePerNight = new List<ulong>();

#if DEBUG
        [Header("temp")]
        public Sprite[] charactersForGame = Array.Empty<Sprite>();
        private GameObject[] _personPrefabs = Array.Empty<GameObject>();
        public long customSeed = -1;
#endif
        private Xoroshiro128Plus _rng;
        private void Awake()
        {
            if(customSeed >= 0)
            {
                _rng = new Xoroshiro128Plus((ulong)customSeed);
            }
            else
            {
                _rng = new Xoroshiro128Plus(VampireNightclubSlayerApplication.instance.applicationRNG);
            }
        }
        private void Start()
        {
#if DEBUG
            GetPersonsForGame();
#endif
            GameObject[] copy = ArrayUtils.Clone(_personPrefabs);
            spawningSpline.Warmup();
            float tForEachCount = 1f / 50;
            for (int i = 0; i < 10; i++)
            {
                spawningSpline.Evaluate(tForEachCount + (tForEachCount * i), out float3 position, out float3 tangent, out float3 upVector);

                var obj = _rng.RetrieveAndRemoveNextElementUniform(ref copy);
                var instance = Instantiate(obj);
                instance.hideFlags = HideFlags.None;
                instance.SetActive(true);
                instance.transform.position = position;
                instance.transform.forward = tangent;
            }
        }

#if DEBUG
        private void GetPersonsForGame()
        {
            List<int> characterUUID = charactersForGame.Select(c => c.name.Split(';')).Select(s => s[2]).Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToList();
            _personPrefabs = characterUUID.Select(PersonCatalog.FindPersonIndex).Select(PersonCatalog.GetPerson).ToArray();
        }
#endif
    }
}