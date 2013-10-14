using System;
using System.Collections.Generic;
using System.Threading;

namespace BicyclesSuite.Shared
{
    public class ItemSet<T> where T : class, IEquatable<T>
    {
        private class ItemWrapper
        {
            public T Item;
            public DateTime Stamp;
        }

        private readonly object _LockObject = new object();
        private readonly int _Timeout;
        private readonly int _StackSize;

        private IList<ItemWrapper> _Container = new List<ItemWrapper>();

        public ItemSet(int timeout = Timeout.Infinite, int stackSize = 0)
        {
            _Timeout = timeout;
            _StackSize = stackSize;
        }

        public bool Contains(T item)
        {
            bool result = false;
            lock (_LockObject)
            {
                int length = _Container.Count;
                for (int i = length - 1; i >= 0; i--)
                {
                    ItemWrapper wrap = _Container[i];
                    if (_StackSize > 0 && _StackSize < i)
                    {
                        _Container.RemoveAt(i);
                        continue;
                    }
                    if (_Timeout != Timeout.Infinite && wrap.Stamp.AddMilliseconds(_Timeout) < DateTime.Now)
                    {
                        _Container.RemoveAt(i);
                        continue;
                    }
                    if (wrap.Item.Equals(item))
                    {
                        result = true;
                        break;
                    }
                }

                if (result == false)
                {
                    _Container.Add(new ItemWrapper { Item = item, Stamp = DateTime.Now });
                }
            }
            return result;
        }
    }
}
