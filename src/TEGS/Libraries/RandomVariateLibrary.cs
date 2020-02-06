// 
// RandomVariateLibrary.cs
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
    [Library(Name = "Random")]
    public class RandomVariateLibrary
    {
        [LibraryConstant]
        public readonly VariableValue Seed;

        private readonly Random _random;

        public RandomVariateLibrary(int? seed = null)
        {
            Seed = new VariableValue(seed ?? GenerateSeed());
            _random = new Random(Seed.IntegerValue);
        }

        private static int GenerateSeed()
        {
            // Adapted from http://lua-users.org/wiki/MathLibraryTutorial
            char[] c = DateTime.UtcNow.Ticks.ToString().ToCharArray();
            Array.Reverse(c);
            return int.Parse(new string(c).Substring(1, 6));
        }

        [LibraryFunction]
        public VariableValue UniformVariate(VariableValue[] args)
        {
            if (args == null || args.Length == 0)
            {
                return new VariableValue(_random.UniformVariate(0, 1));
            }
            else if (args.Length == 2)
            {
                return new VariableValue(_random.UniformVariate(args[0].AsNumber(), args[1].AsNumber()));
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public VariableValue ExponentialVariate(VariableValue[] args)
        {
            if (args != null && args.Length == 1)
            {
                return new VariableValue(_random.ExponentialVariate(args[0].AsNumber()));
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public VariableValue NormalVariate(VariableValue[] args)
        {
            if (args != null && args.Length == 2)
            {
                return new VariableValue(_random.NormalVariate(args[0].AsNumber(), args[1].AsNumber()));
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public VariableValue LogNormalVariate(VariableValue[] args)
        {
            if (args != null && args.Length == 2)
            {
                return new VariableValue(_random.LogNormalVariate(args[0].AsNumber(), args[1].AsNumber()));
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }
    }
}
