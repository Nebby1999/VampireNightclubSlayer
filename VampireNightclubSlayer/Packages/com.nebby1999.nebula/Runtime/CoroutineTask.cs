using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula
{
    public class CoroutineTask : IEnumerator
    {
        public bool isDone => !_internalCoroutine?.MoveNext() ?? true;
        private IEnumerator _internalCoroutine;
        object IEnumerator.Current => _internalCoroutine?.Current;
        bool IEnumerator.MoveNext()
        {
            return _internalCoroutine?.MoveNext() ?? false;
        }

        void IEnumerator.Reset()
        {
            _internalCoroutine?.Reset();
        }

        public CoroutineTask(IEnumerator internalCoroutine)
        {
            _internalCoroutine = internalCoroutine;
        }
    }

    public class ParallelCoroutineTask : IEnumerator
    {

        public bool isDone => !_internalCoroutine.MoveNext();

        private readonly List<IEnumerator> _coroutinesList = new List<IEnumerator>();
        private IEnumerator _internalCoroutine;
        object IEnumerator.Current => _internalCoroutine.Current;

        public ParallelCoroutineTask()
        {
            _internalCoroutine = InternalCoroutine();
        }

        public void Add(IEnumerator coroutine)
        {
            _coroutinesList.Add(coroutine);
        }

        bool IEnumerator.MoveNext()
        {
            return _internalCoroutine.MoveNext();
        }

        void IEnumerator.Reset()
        {
            _internalCoroutine.Reset();
        }

        private IEnumerator InternalCoroutine()
        {
            yield return null;
            bool encounteredUnfinished = true;
            while (encounteredUnfinished)
            {
                encounteredUnfinished = false;
                int i = _coroutinesList.Count - 1;
                while (i >= 0)
                {
                    IEnumerator coroutine = _coroutinesList[i];
                    if (coroutine.MoveNext())
                    {
                        encounteredUnfinished = true;
                        yield return coroutine.Current;
                    }
                    else
                    {
                        _coroutinesList.RemoveAt(i);
                    }
                    int num = i - 1;
                    i = num;
                }
            }
        }
    }
}