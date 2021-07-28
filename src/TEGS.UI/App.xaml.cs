// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using TEGS.UI.ViewModels;
using TEGS.UI.Views;

namespace TEGS.UI
{
    public class App : Application
    {
        public AppViewModel AppVM => AppViewModel.Instance;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Startup += Desktop_Startup;
                desktop.Exit += Desktop_Exit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Startup(object sender, ControlledApplicationLifetimeStartupEventArgs e)
        {
            MessageHandlers.RegisterMessageHandlers(this);

            AppViewModel.Initialize(e.Args);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = new MainWindow
                {
                    VM = new MainViewModel()
                };
                desktop.MainWindow = window;
            }
        }

        private void Desktop_Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            MessageHandlers.UnregisterMessageHandlers(this);
        }
    }
}
