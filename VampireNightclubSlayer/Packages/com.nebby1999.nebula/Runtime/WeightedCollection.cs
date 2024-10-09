using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nebula
{
    public class WeightedCollection<T> : IEnumerable<WeightedCollection<T>.WeightedValue>, ICollection<WeightedCollection<T>.WeightedValue>, IList<WeightedCollection<T>.WeightedValue>
        where T : class
    {
        public WeightedValue this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }
        public int Count => _items.Count;
        public bool IsReadOnly => false;
        public float totalWeight { get; private set; }
        private List<WeightedValue> _items = new List<WeightedValue>();
        internal Xoroshiro128Plus _rng = new Xoroshiro128Plus((ulong)DateTime.Now.Ticks);
        internal ulong? _seed;

        public T Next()
        {
            int index = NextIndex();
            if (index == -1)
                return default;
            return _items[index].value;
        }

        public int NextIndex()
        {
            if (Count == 0)
                return -1;

            float startWeight = _rng.RangeFloat(0, totalWeight);
            for (int i = 0; i < _items.Count; i++)
            {
                startWeight -= _items[i].weight;
                if (startWeight <= 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Add(T value, float weight)
        {
            AddInternal(new WeightedValue(value, weight));
            RecalculateTotalWeight();
        }

        public void Add(WeightedValue item)
        {
            AddInternal(item);
            RecalculateTotalWeight();
        }

        public void Add(IEnumerable<WeightedValue> collection)
        {
            foreach (var value in collection)
            {
                AddInternal(value);
            }
            RecalculateTotalWeight();
        }

        public void SetSeed(ulong seed)
        {
            _seed = seed;
            _rng.ResetSeed(seed);
        }

        private void AddInternal(WeightedValue item)
        {
            _items.Add(item);
        }

        private void RecalculateTotalWeight()
        {
            totalWeight = 0;
            for (int i = 0; i < Count; i++)
            {
                totalWeight += this[i].weight;
            }
        }

        public void Clear()
        {
            _items.Clear();
            RecalculateTotalWeight();
        }

        public bool Contains(WeightedValue item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(WeightedValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WeightedValue> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(WeightedValue item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T value, float weight)
        {
            Insert(index, new WeightedValue(value, weight));
        }

        public void Insert(int index, WeightedValue item)
        {
            _items.Insert(index, item);
            RecalculateTotalWeight();
        }

        public void InsertRange(int index, List<(T, float)> collection)
        {
            InsertRange(index, collection.Select(t => new WeightedValue(t.Item1, t.Item2)));
        }

        public void InsertRange(int index, IEnumerable<WeightedValue> collection)
        {
            _items.InsertRange(index, collection);
            RecalculateTotalWeight();
        }

        public bool Remove(WeightedValue item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            RecalculateTotalWeight();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("WeightedList<");
            stringBuilder.Append(typeof(T).Name);
            stringBuilder.Append(">: TotalWeight:");
            stringBuilder.Append(totalWeight);
            stringBuilder.Append(", Count:");
            stringBuilder.Append(Count);
            stringBuilder.Append(", {\n");
            for (int i = 0; i < Count; i++)
            {
                string s = _items[i].ToString();
                s += i < Count - 1 ? "," : "";
                stringBuilder.AppendLine(s);
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public WeightedCollection()
        {
            _seed = (ulong)DateTime.Now.Ticks;
            _rng = new Xoroshiro128Plus(_seed.Value);
        }

        public WeightedCollection(IEnumerable<WeightedValue> collection) : this()
        {
            Add(collection);
        }

        public WeightedCollection(WeightedCollection<T> orig)
        {
            _seed = orig._seed;
            _rng = orig._rng;
            Add(orig._items);
        }

        public struct WeightedValue : IEquatable<WeightedValue>
        {
            public T value;
            public float weight;

            public override bool Equals(object obj)
            {
                return obj is WeightedValue other && Equals(other);
            }

            public bool Equals(WeightedValue other)
            {
                return value == other.value;
            }

            public static bool operator ==(WeightedValue lhs, WeightedValue rhs)
            {
                return lhs.Equals(rhs);
            }

            public static bool operator !=(WeightedValue lhs, WeightedValue other)
            {
                return !(lhs == other);
            }

            public override int GetHashCode()
            {
                int num1 = value.GetHashCode() / 2;
                int num2 = weight.GetHashCode() / 2;
                return num1 + num2;
            }

            public override readonly string ToString()
            {
                return string.Format("{0} : {1}", weight, value);
            }

            public WeightedValue(T value, float weight)
            {
                this.value = value;
                this.weight = weight;
            }
        }
    }
}