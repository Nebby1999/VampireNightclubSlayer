using Nebula;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

[assembly: SearchableAttribute.OptIn]

namespace VampireSlayer
{
    public class VampireNightclubSlayerApplication : Nebula.ApplicationBehaviour<VampireNightclubSlayerApplication>
    {
        public Xoroshiro128Plus applicationRNG { get; private set; }

        protected override IEnumerator C_LoadGameContent()
        {
            applicationRNG = new Xoroshiro128Plus((ulong)DateTime.Now.Ticks);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var task = SearchableAttribute.Init();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            var coroutineTask = SystemInitializerAttribute.Execute();
            while (!coroutineTask.isDone)
                yield return null;

            ParallelCoroutineTask parallelCoroutineTask = AsyncAssetLoadAttribute.CreateParallelCoroutineTaskForAssembly(GetType().Assembly);
            while (!parallelCoroutineTask.isDone)
                yield return null;

            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds + "ms");
            yield break;
        }
    }
}