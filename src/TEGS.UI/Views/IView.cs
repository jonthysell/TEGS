// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using TEGS.UI.ViewModels;

namespace TEGS.UI.Views
{
    public interface IView<T> where T : ViewModelBase
    {
        T VM { get; set; }
    }
}
