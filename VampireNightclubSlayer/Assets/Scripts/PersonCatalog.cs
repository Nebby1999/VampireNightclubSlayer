using Nebula;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System;
using System.Globalization;

namespace VampireSlayer
{
    public static class PersonCatalog
    {
        public static bool hasCreatedPeople;
        public static int personCount => _personArray.Length;
        private static GameObject[] _personArray = Array.Empty<GameObject>();
        private static int[] _uuids = Array.Empty<int>();
        private static Dictionary<string, PersonIndex> _personNameToIndex = new Dictionary<string, PersonIndex>();
        private static Dictionary<int, PersonIndex> _personUUIDToIndex = new Dictionary<int, PersonIndex>();

        public static ResourceAvailability catalogAvailability;

        public static GameObject GetPerson(PersonIndex index)
        {
            return ArrayUtils.GetSafe(ref _personArray, (int)index);
        }

        public static PersonIndex FindPersonIndex(string personName)
        {
            if(_personNameToIndex.TryGetValue(personName, out var index))
            {
                return index;
            }
            return PersonIndex.Invalid;
        }

        public static PersonIndex FindPersonIndex(int uuid)
        {
            if(_personUUIDToIndex.TryGetValue(uuid, out var index))
            {
                return index;
            }
            return PersonIndex.Invalid;
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
            _personArray = new GameObject[orderedPeople.Count];
            _uuids = new int[orderedPeople.Count];

            GameObject parent = new GameObject("PersonCatalog_Parent");
            GameObject.DontDestroyOnLoad(parent);
            for (int i = 0; i < orderedPeople.Count; i++)
            {
                var personGameObjectInstance = GameObject.Instantiate(personPrefab);
                var spriteToUse = orderedPeople[i];

                string[] splitString = spriteToUse.name.Split(';');

                string firstName = splitString[0];
                string secondName = splitString[1];
                string uuidAsString = splitString[2];

                string personName = $"{firstName} {secondName}";
                int uuid = int.Parse(uuidAsString, CultureInfo.InvariantCulture);

                personGameObjectInstance.name = personName + "GameObject";
                personGameObjectInstance.hideFlags = HideFlags.DontSave;
                var personInstance = personGameObjectInstance.GetComponent<Person>();
                personInstance.personIndex = (PersonIndex)i;
                personInstance.personSprite = spriteToUse;
                personInstance.personName = personName;
                personInstance.personUUID = uuid;
                _personArray[i] = personGameObjectInstance;
                _personNameToIndex[personName] = personInstance.personIndex;
                _personUUIDToIndex[uuid] = personInstance.personIndex;
                _uuids[i] = personInstance.personUUID;

                personGameObjectInstance.transform.SetParent(parent.transform);
            }

            hasCreatedPeople = true;
            VampireNightclubSlayerApplication.OnShutdown += VampireNightclubSlayerApplication_OnShutdown;
            catalogAvailability.MakeAvailable();
        }

        private static void VampireNightclubSlayerApplication_OnShutdown()
        {
            foreach(var obj in _personArray)
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