// 
// Node.cs
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

// Adapted from https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce

using System;

namespace TEGS.Expressions
{
    public abstract class Node
    {
        public abstract VariableValue Evaluate(IContext context);
    }

    public class NodeValue : Node
    {
        public VariableValue Value { get; private set; }

        public NodeValue(VariableValue value) => Value = value;

        public override VariableValue Evaluate(IContext context) => Value;
    }

    #region Context Symbol Resolution

    public class NodeVariable : Node
    {
        public string Name { get; private set; }

        public NodeVariable(string name) => Name = name;

        public override VariableValue Evaluate(IContext context) => context.GetVariable(Name);
    }

    public class NodeFunctionCall : Node
    {
        public string Name { get; private set; }

        public Node[] Arguments { get; private set; }

        public NodeFunctionCall(string name, Node[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public override VariableValue Evaluate(IContext context)
        {
            if (null == Arguments || Arguments.Length == 0)
            {
                return context.CallFunction(Name, null);
            }

            VariableValue[] argumentValues = new VariableValue[Arguments.Length];

            for (int i = 0; i < Arguments.Length; i++)
            {
                argumentValues[i] = Arguments[i].Evaluate(context);
            }

            return context.CallFunction(Name, argumentValues);
        }
    }

    #endregion

    #region Operators

    public abstract class NodeUnary : Node
    {
        public Node RHS { get; private set; }

        public NodeUnary(Node rhs) => RHS = rhs ?? throw new ArgumentNullException(nameof(rhs));
    }

    public class NodeNegative : NodeUnary
    {
        public NodeNegative(Node rhs) : base(rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return -RHS.Evaluate(context);
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    public abstract class NodeBinary : NodeUnary
    {
        public Node LHS { get; private set; }

        public NodeBinary(Node lhs, Node rhs) : base(rhs) => LHS = lhs ?? throw new ArgumentNullException(nameof(lhs));
    }

    public class NodeAddition : NodeBinary
    {
        public NodeAddition(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return LHS.Evaluate(context) + RHS.Evaluate(context);
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    public class NodeSubtraction : NodeBinary
    {
        public NodeSubtraction(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return LHS.Evaluate(context) - RHS.Evaluate(context);
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    public class NodeMultiplication : NodeBinary
    {
        public NodeMultiplication(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return LHS.Evaluate(context) * RHS.Evaluate(context);
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    public class NodeDivision : NodeBinary
    {
        public NodeDivision(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return LHS.Evaluate(context) / RHS.Evaluate(context);
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }


    public class NodeAssign : NodeBinary
    {
        public NodeAssign(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                context.SetVariable((LHS as NodeVariable).Name, RHS.Evaluate(context));
                return LHS.Evaluate(context);
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    public class NodeLessThan : NodeBinary
    {
        public NodeLessThan(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return new VariableValue(LHS.Evaluate(context) < RHS.Evaluate(context));
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    public class NodeGreaterThan : NodeBinary
    {
        public NodeGreaterThan(Node lhs, Node rhs) : base(lhs, rhs) { }

        public override VariableValue Evaluate(IContext context)
        {
            try
            {
                return new VariableValue(LHS.Evaluate(context) > RHS.Evaluate(context));
            }
            catch (ArithmeticException)
            {
                throw new NodeEvaluateException(this);
            }
        }
    }

    #endregion

    #region Exceptions

    public class NodeEvaluateException : Exception
    {
        public Node Node { get; private set; }

        public NodeEvaluateException(Node node) : base()
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }
    }

    #endregion
}
