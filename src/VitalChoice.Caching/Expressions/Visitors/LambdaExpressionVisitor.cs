using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class LambdaExpressionVisitor : ExpressionVisitor
    {
        private readonly Stack<List<Operation>> _operations = new Stack<List<Operation>>();
        private List<Operation> _currentOperations = new List<Operation>();
        private readonly List<Condition> _resultConditions = new List<Condition>();

        public ICollection<Operation> CurrentOperations => _currentOperations;
        public ICollection<Condition> CurrentConditions => _resultConditions;

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
            {
                var leftCondition = new List<Operation>();
                _currentOperations = leftCondition;
                _operations.Push(leftCondition);
                Visit(node.Left);
                _operations.Pop();

                var rightCondition = new List<Operation>();
                _currentOperations = rightCondition;
                _operations.Push(rightCondition);
                Visit(node.Right);
                _operations.Pop();
                _currentOperations = _operations.Count > 0 ? _operations.Peek() : new List<Operation>();
                var condition = new BinaryCondition(ExpressionType.AndAlso);
                condition.Left.AddRange(leftCondition);
                condition.Right.AddRange(rightCondition);
                _resultConditions.Add(condition);
                return node;
            }
            var otherCondition = new BinaryOperation(node.NodeType)
            {
                Left = node.Left,
                Right = node.Right
            };
            _currentOperations.Add(otherCondition);
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            _currentOperations.Add(new Operation(node.NodeType)
            {
                Left = node.Operand
            });
            return node;
        }
    }
}