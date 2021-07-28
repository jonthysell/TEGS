// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using GalaSoft.MvvmLight.Command;

namespace TEGS.UI.ViewModels
{
    public abstract class AcceptRejectViewModelBase : ViewModelBase
    {
        #region Properties

        public bool Result { get; private set; } = false;

        #endregion

        #region Commands

        public RelayCommand Accept
        {
            get
            {
                return _accept ?? (_accept = new RelayCommand(() =>
                {
                    try
                    {
                        ProcessAccept();

                        Result = true;
                        RequestClose();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _accept;

        public RelayCommand Reject
        {
            get
            {
                return _reject ?? (_reject = new RelayCommand(() =>
                {
                    try
                    {
                        ProcessReject();

                        Result = false;
                        RequestClose();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _reject;

        #endregion

        protected AcceptRejectViewModelBase() : base() { }

        protected virtual void ProcessAccept() { }

        protected virtual void ProcessReject() { }
    }
}

