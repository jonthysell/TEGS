// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    public static class BaseLibraries
    {
        public static ILibrary SystemBool => new SystemLibrary(typeof(bool), ReflectionType.StandardConstants);

        public static ILibrary SystemInt => new SystemLibrary(typeof(int), ReflectionType.StandardConstants).Rename("Integer");

        public static ILibrary SystemDouble => new SystemLibrary(typeof(double), ReflectionType.StandardConstants);

        public static ILibrary SystemString => new SystemLibrary(typeof(string), ReflectionType.StandardConstants);

        public static ILibrary StringLibrary => new AttributedLibrary(typeof(StringLibrary));

        public static ILibrary ConvertLibrary => new AttributedLibrary(typeof(ConvertLibrary));

        public static ILibrary SystemMath => new SystemLibrary(typeof(Math), ReflectionType.StandardOnly);

        public static ILibrary RandomVariateLibrary(int? seed = null)
        {
            return new SystemLibrary(seed.HasValue ? new Random(seed.Value) : new Random(), typeof(Random), ReflectionType.ExtensionOnly, typeof(RandomExtensions));
        }

        public static ScriptingHost MakeBaseScriptingHost(int? seed = null)
        {
            ScriptingHost scriptingHost = new ScriptingHost();

            scriptingHost.LoadLibrary(SystemBool);
            scriptingHost.LoadLibrary(SystemInt);
            scriptingHost.LoadLibrary(SystemDouble);
            scriptingHost.LoadLibrary(SystemString);
            scriptingHost.LoadLibrary(StringLibrary);
            scriptingHost.LoadLibrary(ConvertLibrary);
            scriptingHost.LoadLibrary(SystemMath);
            scriptingHost.LoadLibrary(RandomVariateLibrary(seed));

            return scriptingHost;
        }
    }
}
