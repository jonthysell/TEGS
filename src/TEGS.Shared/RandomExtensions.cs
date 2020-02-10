// 
// RandomExtensions.cs
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

namespace TEGS
{
    public static class RandomExtensions
    {
        public static double UniformVariate(this Random random, double alpha, double beta)
        {
            return alpha + (beta - alpha) * random.NextDouble();
        }

        public static double ExponentialVariate(this Random random, double lambda)
        {
            return -Math.Log(1.0 - random.NextDouble()) / lambda;
        }

        public static double NormalVariate(this Random random, double mu, double sigma)
        {
            double z = 0.0;

            while (true)
            {
                double u1 = random.NextDouble();
                double u2 = 1.0 - random.NextDouble();

                z = (4 * Math.Exp(-0.5) / Math.Sqrt(2.0)) * (u1 - 0.5) / u2;

                if ((z * z / 4.0) <= -Math.Log(u2))
                {
                    break;
                }
            }

            return mu + z * sigma;
        }

        public static double LogNormalVariate(this Random random, double mu, double sigma)
        {
            return Math.Exp(random.NormalVariate(mu, sigma));
        }
    }
}
