using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    internal class LambdaExpressionVisitor<T> : ExpressionVisitor
    {
        private readonly Stack<Condition> _conditions = new Stack<Condition>();
        private Condition _condition;

        public Condition Condition
        {
            get
            {
                while (_conditions.Count > 0)
                {
                    _condition = _conditions.Pop();
                }
                return _condition; 
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _conditions.Push(new BinaryCondition(node.NodeType, node));

            var count = _conditions.Count;
            Visit(node.Left);
            var currentLeft = _conditions.Count > count ? _conditions.Pop() : new Condition(node.Left.NodeType, node.Left);

            count = _conditions.Count;
            Visit(node.Right);
            var currentRight = _conditions.Count > count
                ? _conditions.Pop()
                : new Condition(node.Right.NodeType, node.Right);
            while (!(_conditions.Peek() is BinaryCondition))
            {
                _conditions.Pop();
            }
            var topCondition = (BinaryCondition) _conditions.Peek();
            topCondition.Left = currentLeft;
            topCondition.Right = currentRight;
            _condition = topCondition;
            return node;
        }

        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (typeof (T1) == typeof (Func<T, bool>))
            {
                return base.VisitLambda(node);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = base.VisitMethodCall(node);
            if (node.Method.Name == "Contains" && node.Method.DeclaringType.TryGetElementType(typeof (ICollection<>)) != null)
            {
                _condition = new Condition(node.NodeType, node);
                _conditions.Push(_condition);
            }
            if (node.Method.Name == "Equals")
            {
                if (node.Arguments.Count == 2)
                {
                    VisitBinary(Expression.Equal(node.Arguments.First(), node.Arguments.Last()));
                }
                else if (node.Arguments.Count == 1 && node.Object != null)
                {
                    VisitBinary(Expression.Equal(node.Arguments[0], node.Object));
                }
            }
            return result;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            _condition = new Condition(node.NodeType, node);
            _conditions.Push(_condition);
            return base.VisitUnary(node);
        }
    }
}