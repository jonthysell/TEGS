// 
// AttributedLibrary.cs
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
using System.Collections.Generic;
using System.Reflection;

namespace TEGS.Libraries
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LibraryAttribute : Attribute
    {
        public string Name { get; set; } = null;

        public LibraryAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LibraryConstantAttribute : Attribute
    {
        public string Name { get; set; } = null;

        public LibraryConstantAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LibraryFunctionAttribute : Attribute
    {
        public string Name { get; set; } = null;

        public LibraryFunctionAttribute() { }
    }

    public class AttributedLibrary : ILibrary
    {
        #region Properties

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

        protected Dictionary<string, VariableValue> Constants { get; private set; } = new Dictionary<string, VariableValue>();

        protected Dictionary<string, CustomFunction> Functions { get; private set; } = new Dictionary<string, CustomFunction>();

        #endregion

        #region Constructors

        public AttributedLibrary(TypeInfo typeInfo)
        {
            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));

            RegisterName();
            RegisterConstants();
            RegisterCustomFunctions();
        }

        public AttributedLibrary(object instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            TypeInfo = instance.GetType().GetTypeInfo();

            RegisterName();
            RegisterConstants();
            RegisterCustomFunctions();
        }

        #endregion

        #region ILibrary

        public string Name { get; private set; } = "";

        public IEnumerable<KeyValuePair<string, VariableValue>> GetConstants() => Constants;

        public IEnumerable<KeyValuePair<string, CustomFunction>> GetCustomFunctions() => Functions;

        #endregion

        #region Reflection

        private void RegisterName()
        {
            var attribute = TypeInfo.GetCustomAttribute<LibraryAttribute>();
            if (null != attribute)
            {
                Name = attribute.Name?.Trim() ?? "";
            }
        }

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

        #endregion
    }
}
