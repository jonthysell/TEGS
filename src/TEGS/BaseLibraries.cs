// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    public static class BaseLibraries
    {
        public static SystemLibrary SystemBoolean => new SystemLibrary(typeof(bool), ReflectionType.StandardConstants);

        public static SystemLibrary SystemInteger => new SystemLibrary(typeof(int), ReflectionType.StandardConstants).Rename("Integer");

        public static SystemLibrary SystemDouble => new SystemLibrary(typeof(double), ReflectionType.StandardConstants);

        public static SystemLibrary SystemString => new SystemLibrary(typeof(string), ReflectionType.StandardConstants);

        public static AttributedLibrary StringLibrary => new AttributedLibrary(typeof(StringLibrary));

        public static AttributedLibrary ConvertLibrary => new AttributedLibrary(typeof(ConvertLibrary));

        public static SystemLibrary SystemMath => new SystemLibrary(typeof(Math), ReflectionType.StandardOnly);

        public static SystemLibrary RandomVariateLibrary(int? seed = null)
        {
            return new SystemLibrary(seed.HasValue ? new Random(seed.Value) : new Random(), typeof(Random), ReflectionType.ExtensionOnly, typeof(RandomExtensions));
        }

        public static ScriptingHost MakeBaseScriptingHost(int? seed = null)
        {
            ScriptingHost scriptingHost = new ScriptingHost();

            scriptingHost.LoadLibrary(SystemBoolean);
            scriptingHost.LoadLibrary(SystemInteger);
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
