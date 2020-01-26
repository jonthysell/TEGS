// 
// MathLibrary.cs
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
    public class MathLibrary
    {
        [LibraryConstant]
        public static readonly VariableValue PI = new VariableValue(Math.PI);

        [LibraryConstant]
        public static readonly VariableValue E = new VariableValue(Math.E);

        [LibraryFunction]
        public static VariableValue Min(VariableValue[] args)
        {
            if (args != null && args.Length == 2)
            {
                if (args[0].Type == VariableValueType.Integer && args[1].Type == VariableValueType.Integer)
                {
                    return new VariableValue(Math.Min(args[0].IntegerValue, args[1].IntegerValue));
                }
                else if (args[0].IsNumber && args[1].IsNumber)
                {
                    return new VariableValue(Math.Min(args[0].AsNumber(), args[1].AsNumber()));
                }
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public static VariableValue Max(VariableValue[] args)
        {
            if (args != null && args.Length == 2)
            {
                if (args[0].Type == VariableValueType.Integer && args[1].Type == VariableValueType.Integer)
                {
                    return new VariableValue(Math.Max(args[0].IntegerValue, args[1].IntegerValue));
                }
                else if (args[0].IsNumber && args[1].IsNumber)
                {
                    return new VariableValue(Math.Max(args[0].AsNumber(), args[1].AsNumber()));
                }
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }
    }
}
