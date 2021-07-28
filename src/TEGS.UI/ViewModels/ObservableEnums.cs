// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TEGS.UI.ViewModels
{
    public static class ObservableEnums
    {
        private static readonly Dictionary<Type, ObservableCollection<string>> _typesCache = new Dictionary<Type, ObservableCollection<string>>();

        public static ObservableCollection<string> GetCollection<TEnum>()
        {
            if (!_typesCache.TryGetValue(typeof(TEnum), out var result))
            {
                result = new ObservableCollection<string>(Enum.GetNames(typeof(TEnum)));
                _typesCache[typeof(TEnum)] = result;
            }

            return result;
        }
    }
}
