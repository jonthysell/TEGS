// 
// SystemLibrary.cs
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

using TEGS.Expressions;

namespace TEGS.Libraries
{
    public class SystemLibrary : ReflectionLibraryBase
    {
        #region Constructors

        public SystemLibrary(Type type, ReflectionType reflectionType = ReflectionType.All, Type extensions = null) : base(type, reflectionType, extensions) { }

        public SystemLibrary(object instance, ReflectionType reflectionType = ReflectionType.All, Type extensions = null) : base(instance, reflectionType, extensions) { }

        #endregion

        #region ReflectionLibraryBase

        protected override VariableValue GetConstant(FieldInfo fieldInfo) => VariableValue.Parse(fieldInfo.GetValue(Instance));

        protected override VariableValue GetConstant(PropertyInfo propertyInfo) => VariableValue.Parse(propertyInfo.GetMethod.Invoke(Instance, null));

        protected override CustomFunction GetCustomFunction(MethodInfo methodInfo)
        {
            if (!Functions.ContainsKey(methodInfo.Name))
            {
                var returnType = methodInfo.ReturnType;

                if (returnType == typeof(bool) || returnType == typeof(int) || returnType == typeof(double) || returnType == typeof(string))
                {
                    var parameterInfos = methodInfo.GetParameters();
                    if (returnType == typeof(bool) && CheckParams(parameterInfos))
                    {
                        // void => bool
                        var boolFunc = MakeDelegate<Func<bool>>(methodInfo);
                        return MakeFunction(boolFunc);
                    }
                    else if (returnType == typeof(bool) && CheckParams<bool>(parameterInfos))
                    {
                        // bool => bool
                        var boolFunc = MakeDelegate<Func<bool, bool>>(methodInfo);
                        return MakeFunction(boolFunc);
                    }
                    else if (returnType == typeof(int) && CheckParams(parameterInfos))
                    {
                        // void => int
                        var intFunc = MakeDelegate<Func<int>>(methodInfo);
                        return MakeFunction(intFunc);
                    }
                    else if (returnType == typeof(int) && CheckParams<int>(parameterInfos))
                    {
                        // int => int
                        var intFunc = MakeDelegate<Func<int, int>>(methodInfo);

                        if (TryFindRelatedMethod<double, double>(methodInfo, out MethodInfo relatedMethodInfo))
                        {
                            // int => int + double => double
                            var doubleFunc = MakeDelegate<Func<double, double>>(relatedMethodInfo);
                            return MakeFunction(intFunc, doubleFunc);
                        }
                        else
                        {
                            // int => int
                            return MakeFunction(intFunc);
                        }
                    }
                    else if (returnType == typeof(int) && CheckParams<int, int>(parameterInfos))
                    {
                        // int, int => int
                        var intFunc = MakeDelegate<Func<int, int, int>>(methodInfo);

                        if (TryFindRelatedMethod<double, double, double>(methodInfo, out MethodInfo relatedMethodInfo))
                        {
                            // int, int => int + double, double => double
                            var doubleFunc = MakeDelegate<Func<double, double, double>>(relatedMethodInfo);
                            return MakeFunction(intFunc, doubleFunc);
                        }
                        else
                        {
                            // int, int => int
                            return MakeFunction(intFunc);
                        }
                    }
                    else if (returnType == typeof(double) && CheckParams(parameterInfos))
                    {
                        // void => double
                        var doubleFunc = MakeDelegate<Func<double>>(methodInfo);
                        return MakeFunction(doubleFunc);
                    }
                    else if (returnType == typeof(double) && CheckParams<double>(parameterInfos))
                    {
                        // double => double
                        var doubleFunc = MakeDelegate<Func<double, double>>(methodInfo);

                        if (TryFindRelatedMethod<int, int>(methodInfo, out MethodInfo relatedMethodInfo))
                        {
                            // double => double + int => int
                            var intFunc = MakeDelegate<Func<int, int>>(relatedMethodInfo);
                            return MakeFunction(intFunc, doubleFunc);
                        }
                        else
                        {
                            // double => double
                            return MakeFunction(doubleFunc);
                        }
                    }
                    else if (returnType == typeof(double) && CheckParams<double, double>(parameterInfos))
                    {
                        // double, double => double
                        var doubleFunc = MakeDelegate<Func<double, double, double>>(methodInfo);

                        if (TryFindRelatedMethod<int, int, int>(methodInfo, out MethodInfo relatedMethodInfo))
                        {
                            // double, double => double + int, int => int
                            var intFunc = MakeDelegate<Func<int, int, int>>(relatedMethodInfo);
                            return MakeFunction(intFunc, doubleFunc);
                        }
                        else
                        {
                            // double, double => double
                            return MakeFunction(doubleFunc);
                        }
                    }
                    else if (returnType == typeof(string) && CheckParams(parameterInfos))
                    {
                        // void => string
                        var strFunc = MakeDelegate<Func<string>>(methodInfo);
                        return MakeFunction(strFunc);
                    }
                    else if (returnType == typeof(string) && CheckParams<string>(parameterInfos))
                    {
                        var strFunc = MakeDelegate<Func<string, string>>(methodInfo);
                        return MakeFunction(strFunc);
                    }
                    else if (returnType == typeof(string) && CheckParams<string, string>(parameterInfos))
                    {
                        var strFunc = MakeDelegate<Func<string, string, string>>(methodInfo);
                        return MakeFunction(strFunc);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Reflection Helpers

        private bool TryFindRelatedMethod<TArg, TResult>(MethodInfo baseMethodInfo, out MethodInfo result)
        {
            foreach (MethodInfo methodInfo in GetMethods(TypeInfo))
            {
                if (methodInfo != baseMethodInfo && methodInfo.Name == baseMethodInfo.Name)
                {
                    var returnType = methodInfo.ReturnType;

                    if (returnType == typeof(TResult))
                    {
                        var parameterInfo = methodInfo.GetParameters();
                        if (CheckParams<TArg>(parameterInfo))
                        {
                            result = methodInfo;
                            return true;
                        }
                    }
                }
            }

            result = default;
            return false;
        }

        private bool TryFindRelatedMethod<TArg0, TArg1, TResult>(MethodInfo baseMethodInfo, out MethodInfo result)
        {
            foreach (MethodInfo methodInfo in GetMethods(TypeInfo))
            {
                if (methodInfo != baseMethodInfo && methodInfo.Name == baseMethodInfo.Name)
                {
                    var returnType = methodInfo.ReturnType;

                    if (returnType == typeof(TResult))
                    {
                        var parameterInfo = methodInfo.GetParameters();
                        if (CheckParams<TArg0, TArg1>(parameterInfo))
                        {
                            result = methodInfo;
                            return true;
                        }
                    }
                }
            }

            result = default;
            return false;
        }

        private bool CheckParams(ParameterInfo[] parameterInfos)
        {
            return parameterInfos.Length == 0 ||
                (null != Instance && parameterInfos.Length == 1 && parameterInfos[0].ParameterType == TypeInfo);
        }

        private bool CheckParams<TArg>(ParameterInfo[] parameterInfos)
        {
            return (parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(TArg)) ||
                (null != Instance && parameterInfos.Length == 2 && parameterInfos[0].ParameterType == TypeInfo && parameterInfos[1].ParameterType == typeof(TArg));
        }

        private bool CheckParams<TArg0, TArg1>(ParameterInfo[] parameterInfos)
        {
            return (parameterInfos.Length == 2 && parameterInfos[0].ParameterType == typeof(TArg0) && parameterInfos[1].ParameterType == typeof(TArg1)) ||
                (null != Instance && parameterInfos.Length == 3 && parameterInfos[0].ParameterType == TypeInfo && parameterInfos[1].ParameterType == typeof(TArg0) && parameterInfos[2].ParameterType == typeof(TArg1));
        }

        private CustomFunction MakeFunction(Func<bool> func)
        {
            return (args) =>
            {
                if (args == null || args.Length == 0)
                {
                    return new VariableValue(func());
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<bool, bool> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 1 && args[0].Type == VariableValueType.Boolean)
                {
                    return new VariableValue(func(args[0].BooleanValue));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<int> func)
        {
            return (args) =>
            {
                if (args == null || args.Length == 0)
                {
                    return new VariableValue(func());
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<int, int> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 1 && args[0].Type == VariableValueType.Integer)
                {
                    return new VariableValue(func(args[0].IntegerValue));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<int, int, int> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 2 && args[0].Type == VariableValueType.Integer && args[1].Type == VariableValueType.Integer)
                {
                    return new VariableValue(func(args[0].IntegerValue, args[1].IntegerValue));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<double> func)
        {
            return (args) =>
            {
                if (args == null || args.Length == 0)
                {
                    return new VariableValue(func());
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<double, double> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 1 && args[0].IsNumber)
                {
                    return new VariableValue(func(args[0].AsNumber()));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<double, double, double> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 2 && args[0].IsNumber && args[1].IsNumber)
                {
                    return new VariableValue(func(args[0].AsNumber(), args[1].AsNumber()));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<int, int> intFunc, Func<double, double> doubleFunc)
        {
            return (args) =>
            {
                if (args != null && args.Length == 1)
                {
                    if (args[0].Type == VariableValueType.Integer)
                    {
                        return new VariableValue(intFunc(args[0].IntegerValue));
                    }
                    else if (args[0].Type == VariableValueType.Double)
                    {
                        return new VariableValue(doubleFunc(args[0].DoubleValue));
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<int, int, int> intFunc, Func<double, double, double> doubleFunc)
        {
            return (args) =>
            {
                if (args != null && args.Length == 2)
                {
                    if (args[0].Type == VariableValueType.Integer && args[1].Type == VariableValueType.Integer)
                    {
                        return new VariableValue(intFunc(args[0].IntegerValue, args[1].IntegerValue));
                    }
                    else if (args[0].IsNumber && args[1].IsNumber)
                    {
                        return new VariableValue(doubleFunc(args[0].AsNumber(), args[1].AsNumber()));
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<string> func)
        {
            return (args) =>
            {
                if (args == null || args.Length == 0)
                {
                    return new VariableValue(func());
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<string, string> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 1 && args[0].Type == VariableValueType.String)
                {
                    return new VariableValue(func(args[0].StringValue));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        private CustomFunction MakeFunction(Func<string, string, string> func)
        {
            return (args) =>
            {
                if (args != null && args.Length == 2 && args[0].Type == VariableValueType.String && args[1].Type == VariableValueType.String)
                {
                    return new VariableValue(func(args[0].StringValue, args[1].StringValue));
                }

                throw new ArgumentOutOfRangeException(nameof(args));
            };
        }

        #endregion
    }
}
