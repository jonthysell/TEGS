// 
// TableExtensions.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
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

using MoonSharp.Interpreter;

namespace TEGS.Lua
{
    static class TableExtensions
    {
        public static bool HasKey(this Table table, string key)
        {
            return null != table.RawGet(key);
        }

        public static bool TryGet(this Table table, string key, out DynValue value)
        {
            value = table.RawGet(key);
            return (null != value);
        }

        public static bool TryGet(this Table table, string key, out bool value)
        {
            if (table.TryGet(key, out DynValue dynValue) && dynValue.Type == DataType.Boolean)
            {
                value = dynValue.Boolean;
                return true;
            }

            value = default(bool);
            return false;
        }

        public static bool TryGet(this Table table, string key, out int value)
        {
            if (table.TryGet(key, out DynValue dynValue) && dynValue.Type == DataType.Number)
            {
                value = (int)dynValue.Number;
                return true;
            }

            value = default(int);
            return false;
        }

        public static bool TryGet(this Table table, string key, out double value)
        {
            if (table.TryGet(key, out DynValue dynValue) && dynValue.Type == DataType.Number)
            {
                value = dynValue.Number;
                return true;
            }

            value = default(double);
            return false;
        }

        public static bool TryGet(this Table table, string key, out string value)
        {
            if (table.TryGet(key, out DynValue dynValue) && dynValue.Type == DataType.String)
            {
                value = dynValue.String;
                return true;
            }

            value = default(string);
            return false;
        }

        public static bool TrySet(this Table table, string key, DynValue value)
        {
            if (table.HasKey(key))
            {
                table.Set(key, value);
                return true;
            }

            return false;
        }

        public static bool TrySet(this Table table, string key, bool value)
        {
            if (table.HasKey(key))
            {
                table.Set(key, value ? DynValue.True : DynValue.False);
                return true;
            }

            return false;
        }

        public static bool TrySet(this Table table, string key, int value)
        {
            if (table.HasKey(key))
            {
                table.Set(key, DynValue.NewNumber(value));
                return true;
            }

            return false;
        }

        public static bool TrySet(this Table table, string key, double value)
        {
            if (table.HasKey(key))
            {
                table.Set(key, DynValue.NewNumber(value));
                return true;
            }

            return false;
        }

        public static bool TrySet(this Table table, string key, string value)
        {
            if (table.HasKey(key))
            {
                table.Set(key, DynValue.NewString(value));
                return true;
            }

            return false;
        }
    }
}
