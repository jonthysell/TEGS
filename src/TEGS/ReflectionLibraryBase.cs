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

namespace TEGS
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

        protected static readonly Dictionary<TypeInfo, IEnumerable<FieldInfo>> _fieldInfoCache = new Dictionary<TypeInfo, IEnumerable<FieldInfo>>();
        protected static readonly Dictionary<TypeInfo, IEnumerable<PropertyInfo>> _propertyInfoCache = new Dictionary<TypeInfo, IEnumerable<PropertyInfo>>();
        protected static readonly Dictionary<TypeInfo, IEnumerable<MethodInfo>> _methodInfoCache = new Dictionary<TypeInfo, IEnumerable<MethodInfo>>();

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

        public IEnumerable<KeyValuePair<string, VariableValue>> GetConstants()
        {
            return Constants;
        }

        public IEnumerable<KeyValuePair<string, CustomFunction>> GetCustomFunctions()
        {
            return Functions;
        }

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
            foreach (var fieldInfo in GetFields(typeInfo))
            {
                if (fieldInfo.IsPublic && TryGetConstant(fieldInfo, out VariableValue constantValue))
                {
                    Constants.Add(fieldInfo.Name, constantValue);
                }
            }

            foreach (var propertyInfo in GetProperties(typeInfo))
            {
                if (null != propertyInfo.GetGetMethod() && TryGetConstant(propertyInfo, out VariableValue constantValue))
                {
                    Constants.Add(propertyInfo.Name, constantValue);
                }
            }
        }

        protected static IEnumerable<FieldInfo> GetFields(TypeInfo typeInfo)
        {
            if (!_fieldInfoCache.TryGetValue(typeInfo, out IEnumerable<FieldInfo> value))
            {
                value = typeInfo.DeclaredFields;
                _fieldInfoCache[typeInfo] = value;
            }

            return value;
        }

        protected static IEnumerable<PropertyInfo> GetProperties(TypeInfo typeInfo)
        {
            if (!_propertyInfoCache.TryGetValue(typeInfo, out IEnumerable<PropertyInfo> value))
            {
                value = typeInfo.DeclaredProperties;
                _propertyInfoCache[typeInfo] = value;
            }

            return value;
        }

        protected virtual void LoadMethods(TypeInfo typeInfo)
        {
            foreach (var methodInfo in GetMethods(typeInfo))
            {
                if (methodInfo.IsPublic && TryGetCustomFunction(methodInfo, out CustomFunction customFunction))
                {
                    Functions.Add(methodInfo.Name, customFunction);
                }
            }
        }

        protected static IEnumerable<MethodInfo> GetMethods(TypeInfo typeInfo)
        {
            if (!_methodInfoCache.TryGetValue(typeInfo, out IEnumerable<MethodInfo> value))
            {
                value = typeInfo.DeclaredMethods;
                _methodInfoCache[typeInfo] = value;
            }

            return value;
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

        protected virtual VariableValue GetConstant(FieldInfo fieldInfo)
        {
            return (VariableValue)fieldInfo.GetValue(Instance);
        }

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

        protected virtual VariableValue GetConstant(PropertyInfo propertyInfo)
        {
            return (VariableValue)propertyInfo.GetMethod.Invoke(Instance, null);
        }

        protected bool TryGetCustomFunction(MethodInfo methodInfo, out CustomFunction result)
        {
            try
            {
                result = GetCustomFunction(methodInfo);
                return (null != result);
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

            return null;
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
