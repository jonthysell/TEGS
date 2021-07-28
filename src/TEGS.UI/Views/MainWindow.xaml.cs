// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using TEGS.UI.ViewModels;

namespace TEGS.UI.Views
{
    public class MainWindow : Window, IView<MainViewModel>
    {
        public MainViewModel VM
        {
            get
            {
                return (MainViewModel)DataContext;
            }
            set
            {
                DataContext = value;
                value.RequestClose = Close;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
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
