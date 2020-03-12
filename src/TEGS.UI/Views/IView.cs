using System;
using System.Collections.Generic;
using System.Text;

using TEGS.UI.ViewModels;

namespace TEGS.UI.Views
{
    public interface IView<T> where T : ViewModelBase
    {
        T VM { get; set; }
    }
}
