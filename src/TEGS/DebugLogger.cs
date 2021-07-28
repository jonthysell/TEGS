// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace TEGS
{
    public static class DebugLogger
    {
        public static void LogException(Exception ex)
        {
            Debug.WriteLine($"{nameof(DebugLogger)}.{nameof(LogException)}():");
            Debug.Indent();

            string indent = Environment.NewLine + new string(' ', Debug.IndentLevel * Debug.IndentSize);

            Debug.WriteLine(ex.Message.Replace(Environment.NewLine, indent));
            Debug.WriteLine(ex.StackTrace.Replace(Environment.NewLine, indent));

            Debug.Unindent();
        }
    }
}
