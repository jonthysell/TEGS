// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using TEGS.UI.ViewModels;

namespace TEGS.UI.Views
{
    public class GraphStateVariablesWindow : Window, IView<GraphStateVariablesViewModel>
    {
        public GraphStateVariablesViewModel VM
        {
            get
            {
                return (GraphStateVariablesViewModel)DataContext;
            }
            set
            {
                DataContext = value;
                value.RequestClose = Close;
            }
        }

        public GraphStateVariablesWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
