// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TEGS
{
    public abstract class ReflectionLibraryBase : ILibrary
    {
        #region Properties

        protected ReflectionType ReflectionType { get; private set; }

        [DynamicallyAccessedMembers(RequiredMemberTypes)] 
        protected TypeInfo TypeInfo { get; private set; }

        [DynamicallyAccessedMembers(RequiredMemberTypes)] 
        protected TypeInfo ExtensionsTypeInfo { get; private set; }

        protected object Instance { get; private set; } = null;

        protected Dictionary<string, VariableValue> Constants { get; private set; } = new Dictionary<string, VariableValue>();

        protected Dictionary<string, CustomFunction> Functions { get; private set; } = new Dictionary<string, CustomFunction>();

        protected static readonly Dictionary<TypeInfo, IEnumerable<FieldInfo>> _fieldInfoCache = new Dictionary<TypeInfo, IEnumerable<FieldInfo>>();
        protected static readonly Dictionary<TypeInfo, IEnumerable<PropertyInfo>> _propertyInfoCache = new Dictionary<TypeInfo, IEnumerable<PropertyInfo>>();
        protected static readonly Dictionary<TypeInfo, IEnumerable<MethodInfo>> _methodInfoCache = new Dictionary<TypeInfo, IEnumerable<MethodInfo>>();

        #endregion

        #region Constructors

        public ReflectionLibraryBase([DynamicallyAccessedMembers(RequiredMemberTypes)] Type type, ReflectionType reflectionType, [DynamicallyAccessedMembers(RequiredMemberTypes)] Type extensions)
        {
            ReflectionType = reflectionType;
            TypeInfo = type?.GetTypeInfo() ?? throw new ArgumentNullException(nameof(type));
            ExtensionsTypeInfo = extensions?.GetTypeInfo();

            Initialize();
        }

        public ReflectionLibraryBase(object instance, [DynamicallyAccessedMembers(RequiredMemberTypes)] Type type, ReflectionType reflectionType, [DynamicallyAccessedMembers(RequiredMemberTypes)] Type extensions)
        {
            ReflectionType = reflectionType;
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            TypeInfo = type.GetTypeInfo();
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

            if (ExtensionsTypeInfo is not null)
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

        protected virtual void LoadName([DynamicallyAccessedMembers(RequiredMemberTypes)] TypeInfo typeInfo)
        {
            Name = TypeInfo.Name;
        }

        protected virtual void LoadConstants([DynamicallyAccessedMembers(RequiredMemberTypes)] TypeInfo typeInfo)
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
                if (propertyInfo.GetGetMethod() is not null && TryGetConstant(propertyInfo, out VariableValue constantValue))
                {
                    Constants.Add(propertyInfo.Name, constantValue);
                }
            }
        }

        protected static IEnumerable<FieldInfo> GetFields([DynamicallyAccessedMembers(RequiredMemberTypes)] TypeInfo typeInfo)
        {
            if (!_fieldInfoCache.TryGetValue(typeInfo, out IEnumerable<FieldInfo> value))
            {
                value = typeInfo.DeclaredFields;
                _fieldInfoCache[typeInfo] = value;
            }

            return value;
        }

        protected static IEnumerable<PropertyInfo> GetProperties([DynamicallyAccessedMembers(RequiredMemberTypes)] TypeInfo typeInfo)
        {
            if (!_propertyInfoCache.TryGetValue(typeInfo, out IEnumerable<PropertyInfo> value))
            {
                value = typeInfo.DeclaredProperties;
                _propertyInfoCache[typeInfo] = value;
            }

            return value;
        }

        protected virtual void LoadMethods([DynamicallyAccessedMembers(RequiredMemberTypes)] TypeInfo typeInfo)
        {
            foreach (var methodInfo in GetMethods(typeInfo))
            {
                if (methodInfo.IsPublic && TryGetCustomFunction(methodInfo, out CustomFunction customFunction))
                {
                    Functions.Add(methodInfo.Name, customFunction);
                }
            }
        }

        protected static IEnumerable<MethodInfo> GetMethods([DynamicallyAccessedMembers(RequiredMemberTypes)] TypeInfo typeInfo)
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
                return (result is not null);
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
            if (Instance is null)
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), methodInfo);
            }
            else
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), Instance, methodInfo);
            }
        }

        #endregion

        public const DynamicallyAccessedMemberTypes RequiredMemberTypes = DynamicallyAccessedMemberTypes.PublicFields
                                                                        | DynamicallyAccessedMemberTypes.NonPublicFields
                                                                        | DynamicallyAccessedMemberTypes.PublicMethods
                                                                        | DynamicallyAccessedMemberTypes.NonPublicMethods
                                                                        | DynamicallyAccessedMemberTypes.PublicProperties
                                                                        | DynamicallyAccessedMemberTypes.NonPublicProperties;
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
