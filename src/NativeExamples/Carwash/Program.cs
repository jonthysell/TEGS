// 
// Program.cs
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Carwash
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var simArgs = ParseArgs(args);

            Simulation sim = new Simulation();
            sim.Run(simArgs);
        }

        static SimulationArgs ParseArgs(string[] args)
        {
            var simArgs = new SimulationArgs();

            List<int> startValues = new List<int>();

            for (int i = 0; i < args.Length - 1; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--seed":
                        simArgs.Seed = int.Parse(args[++i]);
                        break;
                    case "--start-parameter":
                        startValues.Add(int.Parse(args[++i]));
                        break;
                    case "--stop-time":
                        simArgs.StopCondition.MaxTime = double.Parse(args[++i]);
                        break;
                    default:
                        throw new Exception($"Did not recognize option \"{args[i]}\"");
                }
            }

            simArgs.ParameterValues = startValues.ToArray();

            return simArgs;
        }
    }
}
