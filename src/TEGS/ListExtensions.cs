// 
// ListExtensions.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
