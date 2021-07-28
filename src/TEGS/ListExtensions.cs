// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TEGS
{
    public static class ListExtensions
    {
        public static void SortedInsert<T>(this IList<T> collection, T item) where T : IComparable<T>
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            List<T> tempList = new List<T>(collection);

            int index = Array.BinarySearch(tempList.ToArray(), item);

            if (index < 0)
            {
                index = ~index;
            }

            if (index == collection.Count)
            {
                collection.Add(item);
            }
            else
            {
                collection.Insert(index, item);
            }
        }

        public static bool EqualItems<T>(this IList<T> collection, IList<T> other) where T : IEquatable<T>
        {
            if (collection.Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (!collection[i].Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
