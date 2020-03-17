// 
// GraphPropertiesViewModel.cs
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

namespace TEGS.UI.ViewModels
{
    public class GraphPropertiesViewModel : EditorViewModelBase
    {
        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }
        private string _name;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }
        private string _description;

        public override bool IsDirty => (_name?.Trim() ?? "") != Graph.Name || (_description?.Trim() ?? "") != Graph.Description;

        #endregion

        public ObservableGraph Graph { get; private set; }

        public GraphPropertiesViewModel(ObservableGraph graph) : base("Properties")
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            _name = Graph.Name;
            _description = Graph.Description;
        }

        protected override void ProcessAccept()
        {
            Graph.Name = _name;
            Graph.Description = _description;
        }
    }
}
