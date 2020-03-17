// 
// EditorViewModelBase.cs
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

