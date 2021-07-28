// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace TEGS.UI.ViewModels
{
    public abstract class EditorViewModelBase : AcceptRejectViewModelBase
    {
        #region Properties

        public override string Title => (IsDirty ? "*" : "") + _title;

        private readonly string _title;

        public virtual bool IsDirty => false;

        #endregion

        protected EditorViewModelBase(string title) : base()
        {
            _title = !string.IsNullOrWhiteSpace(title) ? title.Trim() : throw new ArgumentNullException(nameof(title));
            PropertyChanged += EditorViewModelBase_PropertyChanged;
        }

        private void EditorViewModelBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsDirty):
                    RaisePropertyChanged(nameof(Title));
                    break;
            }
        }

        protected void ChildIsDirtyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(IsDirty));
        }
    }
}

