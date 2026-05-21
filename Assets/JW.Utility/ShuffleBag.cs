using System.Collections.Generic;
using System;

namespace JW.Utility
{
    public class ShuffleBag<T>
    {
        private readonly List<T> _sourceItems = new List<T>();
        private readonly List<T> _items = new List<T>();
        private readonly Func<T, float> _weightSelector;
        private Random _rnd;
        private int _seed;
        private int _index;

        public ShuffleBag(List<T> items) : this(items, null) { }

        public ShuffleBag(List<T> items, Func<T, float> weightSelector)
        {
            if (items != null)
                _sourceItems.AddRange(items);

            _weightSelector = weightSelector;

            ReShuffle();
        }
        public T GetItem()
        {
            if (_sourceItems.Count == 0)
                throw new InvalidOperationException("ShuffleBag is empty.");

            if (_index >= _items.Count)
                ReShuffle();

            return _items[_index++];
        }
        private void ReShuffle()
        {
            _items.Clear();
            _items.AddRange(_sourceItems);

            long t = DateTime.UtcNow.Ticks;
            _seed = unchecked((int)(t ^ (t >> 32)));

            _rnd = new Random(_seed);

            if (_weightSelector != null && TryWeightedShuffle())
            {
                _index = 0;
                return;
            }

            ShuffleItems(_items);
            _index = 0;
        }

        private void ShuffleItems(List<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                int j = _rnd.Next(i+1);

                (items[i], items[j]) = (items[j], items[i]);
            }
        }

        private bool TryWeightedShuffle()
        {
            var weightedItems = new List<WeightedItem>(_items.Count);
            var zeroWeightItems = new List<T>();

            for (int i = 0; i < _items.Count; i++)
            {
                T item = _items[i];
                float weight = _weightSelector(item);

                if (float.IsNaN(weight) || weight <= 0)
                {
                    zeroWeightItems.Add(item);
                    continue;
                }

                weightedItems.Add(new WeightedItem(item, CreateWeightedKey(weight)));
            }

            if (weightedItems.Count == 0)
                return false;

            weightedItems.Sort((a, b) => b.Key.CompareTo(a.Key));
            ShuffleItems(zeroWeightItems);

            _items.Clear();

            for (int i = 0; i < weightedItems.Count; i++)
            {
                _items.Add(weightedItems[i].Item);
            }

            _items.AddRange(zeroWeightItems);
            return true;
        }

        private double CreateWeightedKey(float weight)
        {
            double randomValue = _rnd.NextDouble();

            if (randomValue <= 0)
                randomValue = double.Epsilon;

            return Math.Pow(randomValue, 1.0 / weight);
        }

        private readonly struct WeightedItem
        {
            public readonly T Item;
            public readonly double Key;

            public WeightedItem(T item, double key)
            {
                Item = item;
                Key = key;
            }
        }
    }
}
