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

namespace TEGS
{
    public abstract class Node
    {
        public VariableValue Evaluate(IContext context)
        {
            try
            {
                return EvaluateInternal(context);
            }
            catch (NodeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NodeException(this, ex);
            }
        }

        protected abstract VariableValue EvaluateInternal(IContext context);

        public Node Reduce()
        {
            try
            {
                return ReduceInternal();
            }
            catch (NodeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NodeException(this, ex);
            }
        }

        protected virtual Node ReduceInternal()
        {
            return this;
        }
    }

    public class NodeValue : Node
    {
        public VariableValue Value { get; private set; }

        public NodeValue(VariableValue value) => Value = value;

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return Value;
        }
    }

    #region Context Symbol Resolution

    public class NodeVariable : Node
    {
        public string Name { get; private set; }

        public NodeVariable(string name) => Name = name;

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return context.GetVariable(Name);
        }
    }

    public class NodeFunctionCall : Node
    {
        public string Name { get; private set; }

        public Node[] Arguments { get; private set; }

        public VariableValue[] EvaluatedArgs { get; private set; }

        public NodeFunctionCall(string name, Node[] arguments)
        {
            Name = name;
            Arguments = arguments;
            EvaluatedArgs = arguments != null ? new VariableValue[arguments.Length] : null;
        }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            if (null != Arguments)
            {
                for (int i = 0; i < Arguments.Length; i++)
                {
                    EvaluatedArgs[i] = Arguments[i].Evaluate(context);
                }
            }

            return context.CallFunction(Name, EvaluatedArgs);
        }

        protected override Node ReduceInternal()
        {
            if (null != Arguments && Arguments.Length > 0)
            {
                for (int i = 0; i < Arguments.Length; i++)
                {
                    Arguments[i] = Arguments[i].Reduce();
                }
            }

            return this;
        }
    }

    #endregion

    #region Operators

    public abstract class NodeUnary : Node
    {
        public Node RHS { get; private set; }

        public NodeUnary(Node rhs) => RHS = rhs ?? throw new ArgumentNullException(nameof(rhs));

        protected override Node ReduceInternal()
        {
            ReduceRHS();

            return this;
        }

        protected void ReduceRHS()
        {
            RHS = RHS.Reduce();
        }
    }

    public class NodeNegative : NodeUnary
    {
        public NodeNegative(Node rhs) : base(rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return -RHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceRHS();

            if (RHS is NodeValue rhs)
            {
                return new NodeValue(-rhs.Value);
            }

            return this;
        }
    }

    public class NodeNot : NodeUnary
    {
        public NodeNot(Node rhs) : base(rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(!RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceRHS();

            if (RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(!rhs.Value));
            }

            return this;
        }
    }

    public abstract class NodeBinary : NodeUnary
    {
        public Node LHS { get; private set; }

        public NodeBinary(Node lhs, Node rhs) : base(rhs) => LHS = lhs ?? throw new ArgumentNullException(nameof(lhs));

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            return this;
        }

        protected void ReduceLHS()
        {
            LHS = LHS.Reduce();
        }
    }
    public class NodeAssign : NodeBinary
    {
        public NodeAssign(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            context.SetVariable((LHS as NodeVariable).Name, RHS.Evaluate(context));

            return LHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceRHS();

            return this;
        }
    }

    public class NodeAddition : NodeBinary
    {
        public NodeAddition(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return LHS.Evaluate(context) + RHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(lhs.Value + rhs.Value);
            }

            return this;
        }
    }

    public class NodeSubtraction : NodeBinary
    {
        public NodeSubtraction(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return LHS.Evaluate(context) - RHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(lhs.Value - rhs.Value);
            }

            return this;
        }
    }

    public class NodeMultiplication : NodeBinary
    {
        public NodeMultiplication(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return LHS.Evaluate(context) * RHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(lhs.Value * rhs.Value);
            }

            return this;
        }
    }

    public class NodeDivision : NodeBinary
    {
        public NodeDivision(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return LHS.Evaluate(context) / RHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(lhs.Value / rhs.Value);
            }

            return this;
        }
    }

    public class NodeLessThan : NodeBinary
    {
        public NodeLessThan(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) < RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value < rhs.Value));
            }

            return this;
        }
    }

    public class NodeGreaterThan : NodeBinary
    {
        public NodeGreaterThan(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) > RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value > rhs.Value));
            }

            return this;
        }
    }

    public class NodeLessThanEquals : NodeBinary
    {
        public NodeLessThanEquals(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) <= RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value <= rhs.Value));
            }

            return this;
        }
    }

    public class NodeGreaterThanEquals : NodeBinary
    {
        public NodeGreaterThanEquals(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) >= RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value >= rhs.Value));
            }

            return this;
        }
    }

    public class NodeEquals : NodeBinary
    {
        public NodeEquals(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) == RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value == rhs.Value));
            }

            return this;
        }
    }

    public class NodeNotEquals : NodeBinary
    {
        public NodeNotEquals(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) != RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value != rhs.Value));
            }

            return this;
        }
    }

    public class NodeAnd : NodeBinary
    {
        public NodeAnd(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) & RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value & rhs.Value));
            }

            return this;
        }
    }

    public class NodeOr : NodeBinary
    {
        public NodeOr(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return new VariableValue(LHS.Evaluate(context) | RHS.Evaluate(context));
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(new VariableValue(lhs.Value | rhs.Value));
            }

            return this;
        }
    }

    public class NodeConditionalAnd : NodeBinary
    {
        public NodeConditionalAnd(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return LHS.Evaluate(context) ? RHS.Evaluate(context) : VariableValue.False;
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(lhs.Value ? rhs.Value : VariableValue.False);
            }

            return this;
        }
    }

    public class NodeConditionalOr : NodeBinary
    {
        public NodeConditionalOr(Node lhs, Node rhs) : base(lhs, rhs) { }

        protected override VariableValue EvaluateInternal(IContext context)
        {
            return LHS.Evaluate(context) ? VariableValue.True : RHS.Evaluate(context);
        }

        protected override Node ReduceInternal()
        {
            ReduceLHS();
            ReduceRHS();

            if (LHS is NodeValue lhs && RHS is NodeValue rhs)
            {
                return new NodeValue(lhs.Value ? VariableValue.True : rhs.Value);
            }

            return this;
        }
    }

    #endregion

    #region Exceptions

    public class NodeException : Exception
    {
        public Node Node { get; private set; }

        public NodeException(Node node, Exception innerException) : base("", innerException)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }
    }

    #endregion
}
