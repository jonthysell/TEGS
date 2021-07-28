// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using Avalonia.Threading;
using GalaSoft.MvvmLight.Messaging;

namespace TEGS.UI.ViewModels
{
    public static class ExceptionUtils
    {
        public static void HandleException(Exception exception)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Messenger.Default.Send(new ExceptionMessage(exception));
            });
        }
    }
}
