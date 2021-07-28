// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using TEGS.UI.ViewModels;

namespace TEGS.UI.Views
{
    public class GraphPropertiesWindow : Window, IView<GraphPropertiesViewModel>
    {
        public GraphPropertiesViewModel VM
        {
            get
            {
                return (GraphPropertiesViewModel)DataContext;
            }
            set
            {
                DataContext = value;
                value.RequestClose = Close;
            }
        }

        public GraphPropertiesWindow()
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
