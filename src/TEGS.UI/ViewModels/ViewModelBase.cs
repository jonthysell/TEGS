// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using GalaSoft.MvvmLight.Command;

namespace TEGS.UI.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public AppViewModel AppVM => AppViewModel.Instance;

        #region Properties

        public abstract string Title { get; }

        #endregion

        #region Commands

        public RelayCommand NotImplementedCommand
        {
            get
            {
                return _notImplementedCommand ?? (_notImplementedCommand = new RelayCommand(() =>
                {
                    ExceptionUtils.HandleException(new NotImplementedException());
                }, () => {
                    return false;
                }));
            }
        }
        private RelayCommand _notImplementedCommand;

        #endregion

        public Action RequestClose;

        protected ViewModelBase() : base() { }
    }
}
