// 
// BaseLibraries.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace TEGS.Libraries
{
    public static class BaseLibraries
    {
        public static ILibrary SystemBool => new SystemLibrary(typeof(bool), ReflectionType.StandardConstants);

        public static ILibrary SystemInt => new SystemLibrary(typeof(int), ReflectionType.StandardConstants);

        public static ILibrary SystemDouble => new SystemLibrary(typeof(double), ReflectionType.StandardConstants);

        public static ILibrary SystemString => new SystemLibrary(typeof(string), ReflectionType.StandardConstants);

        public static ILibrary StringLibrary => new AttributedLibrary(typeof(StringLibrary));

        public static ILibrary SystemMath => new SystemLibrary(typeof(Math), ReflectionType.StandardOnly);

        public static ILibrary RandomVariateLibrary(int? seed = null) => new SystemLibrary(seed.HasValue ? new Random(seed.Value) : new Random(), ReflectionType.ExtensionOnly, typeof(RandomExtensions));

        public static ScriptingHost MakeBaseScriptingHost(int? seed = null)
        {
            ScriptingHost scriptingHost = new ScriptingHost();

            scriptingHost.LoadLibrary(SystemBool);
            scriptingHost.LoadLibrary(SystemInt);
            scriptingHost.LoadLibrary(SystemDouble);
            scriptingHost.LoadLibrary(SystemString);
            scriptingHost.LoadLibrary(StringLibrary);
            scriptingHost.LoadLibrary(SystemMath);
            scriptingHost.LoadLibrary(RandomVariateLibrary(seed));

            return scriptingHost;
        }
    }
}
