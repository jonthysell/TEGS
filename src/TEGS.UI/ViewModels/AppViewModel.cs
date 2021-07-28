// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using GalaSoft.MvvmLight;

namespace TEGS.UI.ViewModels
{
    public class AppViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Singleton Statics

        public static AppViewModel Instance
        {
            get
            {
                if (null == _instance)
                {
                    Initialize();
                }
                return _instance;
            }
        }
        private static AppViewModel _instance;

        public static void Initialize(string[] args = null)
        {
            if (null != _instance)
            {
                throw new InvalidOperationException($"{ nameof(Instance) } is already initialized.");
            }

            _instance = new AppViewModel();
            if (null != args && args.Length > 0)
            {
                _instance.ParseArgs(args);
            }
        }

        #endregion

        private AppViewModel() { }

        private void ParseArgs(string[] args) { }
    }
}
