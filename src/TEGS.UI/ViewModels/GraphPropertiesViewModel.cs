// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
