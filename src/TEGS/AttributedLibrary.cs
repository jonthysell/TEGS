// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
            if (libraryAttribute is not null)
            {
                Name = libraryAttribute.Name?.Trim() ?? "";
            }
        }

        protected override void LoadConstants(TypeInfo typeInfo)
        {
            foreach (var fieldInfo in GetFields(typeInfo))
            {
                var constantAttribute = fieldInfo.GetCustomAttribute<LibraryConstantAttribute>();
                if (constantAttribute is not null && fieldInfo.IsPublic && TryGetConstant(fieldInfo, out VariableValue constantValue))
                {
                    Constants.Add(constantAttribute.Name ?? fieldInfo.Name, constantValue);
                }
            }

            foreach (var propertyInfo in GetProperties(typeInfo))
            {
                var constantAttribute = propertyInfo.GetCustomAttribute<LibraryConstantAttribute>();
                if (constantAttribute is not null && propertyInfo.GetGetMethod() is not null && TryGetConstant(propertyInfo, out VariableValue constantValue))
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
                if (functionAttribute is not null && methodInfo.IsPublic && TryGetCustomFunction(methodInfo, out CustomFunction customFunction))
                {
                    Functions.Add(functionAttribute.Name ?? methodInfo.Name, customFunction);
                }
            }
        }

        #endregion
    }
}
