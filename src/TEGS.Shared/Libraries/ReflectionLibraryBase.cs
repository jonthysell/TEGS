// 
// ReflectionLibraryBase.cs
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
    public abstract class ReflectionLibraryBase : ILibrary
    {
        #region Properties

        protected ReflectionType ReflectionType { get; private set; }

        protected TypeInfo TypeInfo { get; private set; }

        protected TypeInfo ExtensionsTypeInfo { get; private set; }

        protected object Instance { get; private set; } = null;

        protected Dictionary<string, VariableValue> Constants { get; private set; } = new Dictionary<string, VariableValue>();

        protected Dictionary<string, CustomFunction> Functions { get; private set; } = new Dictionary<string, CustomFunction>();

        #endregion

        #region Constructors

        public ReflectionLibraryBase(Type type, ReflectionType reflectionType, Type extensions)
        {
            ReflectionType = reflectionType;
            TypeInfo = type?.GetTypeInfo() ?? throw new ArgumentNullException(nameof(type));
            ExtensionsTypeInfo = extensions?.GetTypeInfo();

            Initialize();
        }

        public ReflectionLibraryBase(object instance, ReflectionType reflectionType, Type extensions)
        {
            ReflectionType = reflectionType;
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            TypeInfo = instance.GetType().GetTypeInfo();
            ExtensionsTypeInfo = extensions?.GetTypeInfo();

            Initialize();
        }

        #endregion

        #region ILibrary

        public string Name { get; protected set; } = "";

        public IEnumerable<KeyValuePair<string, VariableValue>> GetConstants() => Constants;

        public IEnumerable<KeyValuePair<string, CustomFunction>> GetCustomFunctions() => Functions;

        #endregion

        #region Reflection

        protected void Initialize()
        {
            LoadName(TypeInfo);

            if (ReflectionType.HasFlag(ReflectionType.StandardConstants))
            {
                LoadConstants(TypeInfo);
            }

            if (ReflectionType.HasFlag(ReflectionType.StandardMethods))
            {
                LoadMethods(TypeInfo);
            }

            if (null != ExtensionsTypeInfo)
            {
                if (ReflectionType.HasFlag(ReflectionType.ExtensionConstants))
                {
                    LoadConstants(ExtensionsTypeInfo);
                }

                if (ReflectionType.HasFlag(ReflectionType.ExtensionMethods))
                {
                    LoadMethods(ExtensionsTypeInfo);
                }
            }
        }

        protected virtual void LoadName(TypeInfo typeInfo)
        {
            Name = TypeInfo.Name;
        }

        protected virtual void LoadConstants(TypeInfo typeInfo)
        {
            foreach (var fieldInfo in typeInfo.DeclaredFields)
            {
                if (fieldInfo.IsPublic && TryGetConstant(fieldInfo, out VariableValue constantValue))
                {
                    Constants.Add(fieldInfo.Name, constantValue);
                }
            }

            foreach (var propertyInfo in typeInfo.DeclaredProperties)
            {
                if (null != propertyInfo.GetGetMethod() && TryGetConstant(propertyInfo, out VariableValue constantValue))
                {
                    Constants.Add(propertyInfo.Name, constantValue);
                }
            }
        }

        protected virtual void LoadMethods(TypeInfo typeInfo)
        {
            foreach (var methodInfo in typeInfo.DeclaredMethods)
            {
                if (methodInfo.IsPublic && TryGetCustomFunction(methodInfo, out CustomFunction customFunction))
                {
                    Functions.Add(methodInfo.Name, customFunction);
                }
            }
        }

        protected bool TryGetConstant(FieldInfo fieldInfo, out VariableValue result)
        {
            try
            {
                result = GetConstant(fieldInfo);
                return true;
            }
            catch (Exception) { }

            result = default;
            return false;
        }

        protected virtual VariableValue GetConstant(FieldInfo fieldInfo) => (VariableValue)fieldInfo.GetValue(Instance);

        protected bool TryGetConstant(PropertyInfo propertyInfo, out VariableValue result)
        {
            try
            {
                result = GetConstant(propertyInfo);
                return true;
            }
            catch (Exception) { }

            result = default;
            return false;
        }

        protected virtual VariableValue GetConstant(PropertyInfo propertyInfo) => (VariableValue)propertyInfo.GetMethod.Invoke(Instance, null);

        protected bool TryGetCustomFunction(MethodInfo methodInfo, out CustomFunction result)
        {
            try
            {
                result = GetCustomFunction(methodInfo);
                return true;
            }
            catch (Exception) { }

            result = default;
            return false;
        }

        protected virtual CustomFunction GetCustomFunction(MethodInfo methodInfo)
        {
            var returnType = methodInfo.ReturnType;
            var parameterInfo = methodInfo.GetParameters();

            if (returnType == typeof(VariableValue) &&
                parameterInfo.Length == 1 &&
                parameterInfo[0].ParameterType == typeof(VariableValue[]))
            {
                // Method already matches CustomFunction type
                return MakeDelegate<CustomFunction>(methodInfo);
            }

            throw new ArgumentOutOfRangeException(nameof(methodInfo));
        }

        protected TDelegate MakeDelegate<TDelegate>(MethodInfo methodInfo) where TDelegate: Delegate
        {
            if (null == Instance)
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), methodInfo);
            }
            else
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), Instance, methodInfo);
            }
        }

        #endregion
    }

    [Flags]
    public enum ReflectionType
    {
        None = 0x0,
        StandardConstants = 0x1,
        StandardMethods = 0x2,
        ExtensionConstants = 0x4,
        ExtensionMethods = 0x8,
        StandardOnly = StandardConstants + StandardMethods,
        ExtensionOnly = ExtensionConstants + ExtensionMethods,
        All = StandardOnly + ExtensionMethods,
    }
}
