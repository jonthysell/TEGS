// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        protected static ObservableType Create<ObservableType, InternalType>(InternalType item, Func<InternalType, ObservableType> create, IsDirtyChangedEventHandler onIsDirtyChanged) where ObservableType : ObservableObject<InternalType>, IComparable<ObservableObject<InternalType>>, IEquatable<ObservableObject<InternalType>> where InternalType : IComparable<InternalType>, IEquatable<InternalType>, ICloneable<InternalType>
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));

            }

            if (null == create)
            {
                throw new ArgumentNullException(nameof(create));

            }

            var observableItem = create(item);

            if (null != onIsDirtyChanged)
            {
                observableItem.IsDirtyChanged += onIsDirtyChanged;
            }

            return observableItem;
        }

        protected static ObservableType CreateNew<ObservableType, InternalType>(Func<ObservableType> createNew, IsDirtyChangedEventHandler onIsDirtyChanged) where ObservableType : ObservableObject<InternalType>, IComparable<ObservableObject<InternalType>>, IEquatable<ObservableObject<InternalType>> where InternalType : IComparable<InternalType>, IEquatable<InternalType>, ICloneable<InternalType>
        {
            if (null == createNew)
            {
                throw new ArgumentNullException(nameof(createNew));
            }

            var observableItem = createNew();

            if (null != onIsDirtyChanged)
            {
                observableItem.IsDirtyChanged += onIsDirtyChanged;
            }

            return observableItem;
        }

        protected static ObservableCollection<ObservableType> MakeObservableCollection<ObservableType, InternalType>(ICollection<InternalType> source, Func<InternalType, ObservableType> create, bool clone, IsDirtyChangedEventHandler onIsDirtyChanged) where ObservableType : ObservableObject<T>, IComparable<ObservableType>, IEquatable<ObservableType> where InternalType : IComparable<InternalType>, IEquatable<InternalType>, ICloneable<InternalType>
        {
            if (null == source)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (null == create)
            {
                throw new ArgumentNullException(nameof(create));
            }

            var target = new ObservableCollection<ObservableType>();

            foreach (var item in source)
            {
                var observableItem = create(clone ? item.Clone() : item);
                target.SortedInsert(observableItem);

                if (null != onIsDirtyChanged)
                {
                    observableItem.IsDirtyChanged += onIsDirtyChanged;
                }
            }

            return target;
        }

        protected void SaveToOriginal()
        {
            OriginalInternalObject = InternalObject.Clone();
            RaisePropertyChanged(nameof(IsDirty));
        }

        protected void ReloadFromOriginal()
        {
            InternalObject = OriginalInternalObject.Clone();
            RaisePropertyChanged(nameof(IsDirty));
        }

        #endregion

        public bool Equals(ObservableObject<T> other)
        {
            return InternalObject.Equals(other.InternalObject);
        }

        public int CompareTo(ObservableObject<T> other)
        {
            return InternalObject.CompareTo(other.InternalObject);
        }

        public override bool Equals(object obj)
        {
            if (obj is ObservableObject<T> other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return InternalObject.GetHashCode();
        }
    }
}
