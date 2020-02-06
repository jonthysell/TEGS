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

namespace TEGS.Libraries
{
    public class SystemLibrary : ReflectionLibraryBase
    {
        #region Constructors

        public SystemLibrary(Type type) : base(type) { }

        public SystemLibrary(object instance) : base(instance) { }

        #endregion

        #region ReflectionLibraryBase

        protected override VariableValue GetConstant(FieldInfo fieldInfo) => VariableValue.Parse(fieldInfo.GetValue(Instance));

        protected override VariableValue GetConstant(PropertyInfo propertyInfo) => VariableValue.Parse(propertyInfo.GetMethod.Invoke(Instance, null));

        protected override CustomFunction GetCustomFunction(MethodInfo methodInfo)
        {
            if (!Functions.ContainsKey(methodInfo.Name))
            {
                var returnType = methodInfo.ReturnType;
                var parameterInfo = methodInfo.GetParameters();

                if (returnType == typeof(bool) &&
                    parameterInfo.Length == 1 &&
                    parameterInfo[0].ParameterType == typeof(bool))
                {
                    // bool => bool
                    var boolFunc = MakeDelegate<Func<bool, bool>>(methodInfo);
                    return MakeFunction(boolFunc);
                }
                else if (returnType == typeof(int) &&
                    parameterInfo.Length == 1 &&
                    parameterInfo[0].ParameterType == typeof(int))
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
                else if (returnType == typeof(int) &&
                    parameterInfo.Length == 2 &&
                    parameterInfo[0].ParameterType == typeof(int) &&
                    parameterInfo[1].ParameterType == typeof(int))
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
                else if (returnType == typeof(double) &&
                    parameterInfo.Length == 1 &&
                    parameterInfo[0].ParameterType == typeof(double))
                {
                    // double => double
                    var doubleFunc = (Func<double, double>)methodInfo.CreateDelegate(typeof(Func<double, double>));

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
                else if (returnType == typeof(double) &&
                    parameterInfo.Length == 2 &&
                    parameterInfo[0].ParameterType == typeof(double) &&
                    parameterInfo[1].ParameterType == typeof(double))
                {
                    // double, double => double
                    var doubleFunc = (Func<double, double, double>)methodInfo.CreateDelegate(typeof(Func<double, double, double>));

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
                else if (returnType == typeof(string) &&
                    parameterInfo.Length == 1 &&
                    parameterInfo[0].ParameterType == typeof(string))
                {
                    var strFunc = MakeDelegate<Func<string, string>>(methodInfo);
                    return MakeFunction(strFunc);
                }
            }

            throw new ArgumentOutOfRangeException(nameof(methodInfo));
        }

        #endregion

        #region Reflection Helpers

        private bool TryFindRelatedMethod<TArg, TResult>(MethodInfo baseMethodInfo, out MethodInfo result)
        {
            foreach (MethodInfo methodInfo in TypeInfo.DeclaredMethods)
            {
                if (methodInfo != baseMethodInfo && methodInfo.Name == baseMethodInfo.Name)
                {
                    var returnType = methodInfo.ReturnType;
                    var parameterInfo = methodInfo.GetParameters();

                    if (returnType == typeof(TResult) &&
                        parameterInfo.Length == 1 &&
                        parameterInfo[0].ParameterType == typeof(TArg))
                    {
                        result = methodInfo;
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }

        private bool TryFindRelatedMethod<TArg0, TArg1, TResult>(MethodInfo baseMethodInfo, out MethodInfo result)
        {
            foreach (MethodInfo methodInfo in TypeInfo.DeclaredMethods)
            {
                if (methodInfo != baseMethodInfo && methodInfo.Name == baseMethodInfo.Name)
                {
                    var returnType = methodInfo.ReturnType;
                    var parameterInfo = methodInfo.GetParameters();

                    if (returnType == typeof(TResult) &&
                        parameterInfo.Length == 2 &&
                        parameterInfo[0].ParameterType == typeof(TArg0) &&
                        parameterInfo[1].ParameterType == typeof(TArg1))
                    {
                        result = methodInfo;
                        return true;
                    }
                }
            }

            result = default;
            return false;
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

        #endregion
    }
}
