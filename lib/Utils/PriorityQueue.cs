using System.Collections.Generic;

namespace Bingo.Graph
{
    class PriorityQueue<T>
    {
        private readonly BinaryHeap<T> _queue;

        public PriorityQueue()
        {
            _queue = new BinaryHeap<T>();
        }

        public void Enqueue(double priority, T element)
        {
            _queue.Add(priority, element);
        }

        public T Dequeue()
        {
            return _queue.RemoveMax();
        }

        public double GetMaxPriority()
        {
            return _queue.MaxKey();
        }

        public bool Empty
        {
            get { return _queue.Count == 0; }
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }


    class BinaryHeap<T>
    {
        private readonly List<KeyValuePair<double, T>> _list = new List<KeyValuePair<double, T>> { new KeyValuePair<double, T>() };
        private int _listLength = 1;

        public BinaryHeap()
        {
            _list = new List<KeyValuePair<double, T>> {new KeyValuePair<double, T>()};
        }

        private void AddToList(KeyValuePair<double, T> pair)
        {
            if (_listLength == _list.Count)
            {
                _list.Add(pair);
                _listLength = _list.Count;
            }
            else
            {
                _list[_listLength++] = pair;
            }
        }

        private void Normalize(int idx, KeyValuePair<double, T> obj)
        {
            while (idx > 1)
            {
                var next = idx >> 1;
                if (obj.Key <= _list[next].Key)
                    break;
                _list[idx] = _list[next];
                idx = next;
            }
            _list[idx] = obj;
        }

        public void Add(double key, T value)
        {
            int idx = _listLength;
            var pair = new KeyValuePair<double, T>(key, value);
            AddToList(pair);
            Normalize(idx, pair);
        }

        public T Max()
        {
            return _list[1].Value;
        }

        public double MaxKey()
        {
            return _list[1].Key;
        }

        public T RemoveMax()
        {
            var result = _list[1];

            var temp = _list[_listLength - 1];
            --_listLength;

            int idx = 1;
            var left = idx << 1;
            var right = left + 1;
            bool breaked = false;

            while (right < _listLength)
            {
                if (_list[right].Key >= _list[left].Key)
                {
                    if (_list[right].Key <= temp.Key)
                    {
                        breaked = true;
                        break;
                    }
                    _list[idx] = _list[right];
                    idx = right;
                }
                else
                {
                    if (_list[left].Key <= temp.Key)
                    {
                        breaked = true;
                        break;
                    }
                    _list[idx] = _list[left];
                    idx = left;
                }

                left = idx << 1;
                right = left + 1;
            }

            if (left < _listLength && temp.Key < _list[left].Key && !breaked)
            {
                _list[idx] = _list[left];
                idx = left;
            }

            if (_listLength > 1)
                _list[idx] = temp;

            return result.Value;
        }

        public int Count
        {
            get { return _listLength - 1; }
        }

        public void Clear()
        {
            _list.Clear();
            _list.Add(new KeyValuePair<double, T>());
            _listLength = 1;
        }

        public bool Empty
        {
            get { return _listLength == 1; }
        }
    }
}
