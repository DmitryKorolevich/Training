using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class LambdaExpressionVisitor<T> : ExpressionVisitor
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
            //if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
            //{
            _conditions.Push(new BinaryCondition(node.NodeType));

            var count = _conditions.Count;
            Visit(node.Left);
            var currentLeft = _conditions.Count > count ? _conditions.Pop() : new Condition(node.Left.NodeType) {Expression = node.Left};

            count = _conditions.Count;
            Visit(node.Right);
            var currentRight = _conditions.Count > count
                ? _conditions.Pop()
                : new Condition(node.Right.NodeType) {Expression = node.Right};

            var topCondition = (BinaryCondition) _conditions.Pop();
            topCondition.Left = currentLeft;
            topCondition.Right = currentRight;
            _condition = topCondition;
            //}
            //else
            //{
            //    Condition = new BinaryCondition(node.NodeType)
            //    {
            //        Left = new Condition(node.Left.NodeType) {Expression = node.Left},
            //        Right = new Condition(node.Right.NodeType) {Expression = node.Right}
            //    };
            //    _conditions.Push(Condition);
            //}
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
            if ((node.Method.DeclaringType == typeof (Enumerable) || node.Method.DeclaringType.TryGetElementType(typeof (ICollection<>)) != null) &&
                node.Method.Name == "Contains")
            {
                _condition = new Condition(node.NodeType)
                {
                    Expression = node
                };
                _conditions.Push(_condition);
            }
            return result;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType != ExpressionType.Convert && node.NodeType != ExpressionType.ConvertChecked)
            {
                _condition = new Condition(node.NodeType)
                {
                    Expression = node
                };
                _conditions.Push(_condition);
            }
            return base.VisitUnary(node);
        }
    }
}