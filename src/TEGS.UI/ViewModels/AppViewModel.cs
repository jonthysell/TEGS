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
                if (_instance is null)
                {
                    Initialize();
                }
                return _instance;
            }
        }
        private static AppViewModel _instance;

        public static void Initialize(string[] args = null)
        {
            if (_instance is not null)
            {
                throw new InvalidOperationException($"{ nameof(Instance) } is already initialized.");
            }

            _instance = new AppViewModel();
            if (args is not null && args.Length > 0)
            {
                ParseArgs(args);
            }
        }

        #endregion

        private AppViewModel() { }

        private static void ParseArgs(string[] _) { }
    }
}
