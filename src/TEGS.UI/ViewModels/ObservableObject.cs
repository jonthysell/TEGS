// 
// ObservableObject.cs
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
using System.ComponentModel;

namespace TEGS.UI.ViewModels
{
    public delegate void IsDirtyChangedEventHandler(object sender, EventArgs e);

    public abstract class ObservableObject<T> : GalaSoft.MvvmLight.ObservableObject, IComparable<ObservableObject<T>>, IEquatable<ObservableObject<T>> where T : IComparable<T>, IEquatable<T>, ICloneable<T>
    {
        #region Properties

        public virtual bool IsDirty => !InternalObject.Equals(OriginalInternalObject);

        #endregion

        internal T InternalObject { get; private set; }

        internal T OriginalInternalObject { get; private set; }

        public event IsDirtyChangedEventHandler IsDirtyChanged;

        #region Creation

        protected ObservableObject(T @object)
        {
            OriginalInternalObject = @object ?? throw new ArgumentNullException(nameof(@object));
            InternalObject = OriginalInternalObject.Clone();

            PropertyChanged += ObservableObject_PropertyChanged;
        }

        private void ObservableObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDirty))
            {
                IsDirtyChanged?.Invoke(this, null);
            }
        }

        #endregion

        public bool Equals(ObservableObject<T> other) => InternalObject.Equals(other.InternalObject);

        public int CompareTo(ObservableObject<T> other) => InternalObject.CompareTo(other.InternalObject);

    }
}
