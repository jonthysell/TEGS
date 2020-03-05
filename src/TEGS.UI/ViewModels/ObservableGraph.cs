// 
// ObservableGraph.cs
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
using System.IO;

using GalaSoft.MvvmLight;

namespace TEGS.UI.ViewModels
{
    public class ObservableGraph : ObservableObject
    {
        public string FileName { get; private set; }

        internal Graph Graph { get; private set; }

        private ObservableGraph(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        public static ObservableGraph NewGraph() => new ObservableGraph(new Graph());

        public static ObservableGraph OpenGraph(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            filename = Path.GetFullPath(filename);

            using var fs = new FileStream(filename, FileMode.Open);

            return new ObservableGraph(Graph.LoadXml(fs))
            {
                FileName = filename
            };
        }
    }
}
