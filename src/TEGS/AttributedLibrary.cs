﻿// 
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
using System.Reflection;

namespace TEGS
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

    public class AttributedLibrary : ReflectionLibraryBase
    {
        #region Constructors

        public AttributedLibrary(Type type) : base(type, ReflectionType.All, null) { }

        public AttributedLibrary(object instance) : base(instance, ReflectionType.All, null) { }

        #endregion

        #region ReflectionLibraryBase

        protected override void LoadName(TypeInfo typeInfo)
        {
            var libraryAttribute = TypeInfo.GetCustomAttribute<LibraryAttribute>();
            if (null != libraryAttribute)
            {
                Name = libraryAttribute.Name?.Trim() ?? "";
            }
        }

        protected override void LoadConstants(TypeInfo typeInfo)
        {
            foreach (var fieldInfo in GetFields(typeInfo))
            {
                var constantAttribute = fieldInfo.GetCustomAttribute<LibraryConstantAttribute>();
                if (null != constantAttribute && fieldInfo.IsPublic && TryGetConstant(fieldInfo, out VariableValue constantValue))
                {
                    Constants.Add(constantAttribute.Name ?? fieldInfo.Name, constantValue);
                }
            }

            foreach (var propertyInfo in GetProperties(typeInfo))
            {
                var constantAttribute = propertyInfo.GetCustomAttribute<LibraryConstantAttribute>();
                if (null != constantAttribute && null != propertyInfo.GetGetMethod() && TryGetConstant(propertyInfo, out VariableValue constantValue))
                {
                    Constants.Add(constantAttribute.Name ?? propertyInfo.Name, constantValue);
                }
            }
        }

        protected override void LoadMethods(TypeInfo typeInfo)
        {
            foreach (var methodInfo in GetMethods(typeInfo))
            {
                var functionAttribute = methodInfo.GetCustomAttribute<LibraryFunctionAttribute>();
                if (null != functionAttribute && methodInfo.IsPublic && TryGetCustomFunction(methodInfo, out CustomFunction customFunction))
                {
                    Functions.Add(functionAttribute.Name ?? methodInfo.Name, customFunction);
                }
            }
        }

        #endregion
    }
}
