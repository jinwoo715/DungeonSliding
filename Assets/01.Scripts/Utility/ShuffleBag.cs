using System.Collections.Generic;
using System;

namespace JW.Utility
{
    public class ShuffleBag<T>
    {
        private readonly List<T> _items = new List<T>();
        private Random _rnd;
        private int _seed;
        private int _index;

        public ShuffleBag(List<T> items)
        {
            _items = items;

            ReShuffle();
        }
        public T GetItem()
        {
            if (_index >= _items.Count)
                ReShuffle();

            return _items[_index++];
        }
        private void ReShuffle()
        {
            long t = DateTime.UtcNow.Ticks;
            _seed = unchecked((int)(t ^ (t >> 32)));

            _rnd = new Random(_seed);

            for (int i = _items.Count - 1; i >= 0; i--)
            {
                int j = _rnd.Next(i+1);

                (_items[i], _items[j]) = (_items[j], _items[i]);
            }

            _index = 0;
        }
    }
}
