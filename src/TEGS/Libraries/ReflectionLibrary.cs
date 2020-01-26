// 
// ReflectionLibrary.cs
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
using System.Reflection;

namespace TEGS.Libraries
{
    public class ReflectionLibrary : StaticLibrary
    {
        public TypeInfo TypeInfo
        {
            get
            {
                return _typeInfo ?? (_typeInfo = GetType().GetTypeInfo());
            }
            private set
            {
                _typeInfo = value;
            }
        }
        private TypeInfo _typeInfo = null;

        public object Instance { get; private set; } = null;

        protected ReflectionLibrary()
        {
            RegisterConstants();
            RegisterCustomFunctions();
        }

        private ReflectionLibrary(TypeInfo typeInfo)
        {
            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));

            RegisterConstants();
            RegisterCustomFunctions();
        }

        private ReflectionLibrary(object instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            TypeInfo = instance.GetType().GetTypeInfo();

            RegisterConstants();
            RegisterCustomFunctions();
        }

        public static ReflectionLibrary Create(Type type) => new ReflectionLibrary(type.GetTypeInfo());

        public static ReflectionLibrary Create(object instance) => new ReflectionLibrary(instance);

        private void RegisterConstants()
        {
            foreach (var fieldInfo in TypeInfo.DeclaredFields)
            {
                var attribute = fieldInfo.GetCustomAttribute<LibraryConstantAttribute>();
                if (null != attribute)
                {
                    Constants.Add(attribute.Name ?? fieldInfo.Name, (VariableValue)fieldInfo.GetValue(Instance));
                }
            }

            foreach (var propertyInfo in TypeInfo.DeclaredProperties)
            {
                var attribute = propertyInfo.GetCustomAttribute<LibraryConstantAttribute>();
                if (null != attribute)
                {
                    Constants.Add(attribute.Name ?? propertyInfo.Name, (VariableValue)propertyInfo.GetMethod.Invoke(Instance, null));
                }
            }
        }

        private void RegisterCustomFunctions()
        {
            foreach (var methodInfo in TypeInfo.DeclaredMethods)
            {
                var attribute = methodInfo.GetCustomAttribute<LibraryFunctionAttribute>();
                if (null != attribute)
                {
                    string name = attribute.Name ?? methodInfo.Name;

                    if (null == Instance)
                    {
                        Functions.Add(name, (CustomFunction)Delegate.CreateDelegate(typeof(CustomFunction), methodInfo));
                    }
                    else
                    {
                        Functions.Add(name, (CustomFunction)Delegate.CreateDelegate(typeof(CustomFunction), Instance, methodInfo));
                    }
                }
            }
        }
    }
}
