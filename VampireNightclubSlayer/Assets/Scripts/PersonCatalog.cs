using Nebula;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System;

namespace VampireSlayer
{
    public static class PersonCatalog
    {
        public static bool hasCreatedPeople;
        public static int personCount => personArray.Length;
        private static GameObject[] personArray = Array.Empty<GameObject>();
        public static ResourceAvailability catalogAvailability;

        public static GameObject GetPerson(PersonIndex index)
        {
            return ArrayUtils.GetSafe(ref personArray, (int)index);
        }
        [SystemInitializer]
        private static IEnumerator Initialize()
        {
            var peopleAsyncOp = Addressables.LoadAssetsAsync<Sprite>("PeopleSprites", null);
            var personPrefabOp = Addressables.LoadAssetAsync<GameObject>("GameAssets/Person.prefab");

            ParallelCoroutineTask parallelCoroutine = new ParallelCoroutineTask();
            parallelCoroutine.Add(peopleAsyncOp);
            parallelCoroutine.Add(personPrefabOp);

            while (!parallelCoroutine.isDone)
            {
                yield return null;
            }

            var personPrefab = personPrefabOp.Result;
            personPrefab.SetActive(false);
            var orderedPeople = peopleAsyncOp.Result.OrderBy(p => p.name).ToList();
            personArray = new GameObject[orderedPeople.Count];

            GameObject parent = new GameObject("PersonCatalog_Parent");
            GameObject.DontDestroyOnLoad(parent);
            for (int i = 0; i < orderedPeople.Count; i++)
            {
                var personGameObjectInstance = GameObject.Instantiate(personPrefab);
                var spriteToUse = orderedPeople[i];

                personGameObjectInstance.name = spriteToUse.name + "GameObject";
                personGameObjectInstance.hideFlags = HideFlags.DontSave;
                var personInstance = personGameObjectInstance.GetComponent<Person>();
                personInstance.personIndex = (PersonIndex)i;
                personInstance.personSprite = spriteToUse;
                personArray[i] = personGameObjectInstance;
                personGameObjectInstance.transform.SetParent(parent.transform);
            }

            hasCreatedPeople = true;
            VampireNightclubSlayerApplication.OnShutdown += VampireNightclubSlayerApplication_OnShutdown;
            catalogAvailability.MakeAvailable();
        }

        private static void VampireNightclubSlayerApplication_OnShutdown()
        {
            foreach(var obj in personArray)
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }

    public enum PersonIndex
    {
        Invalid = -1
    }
}