using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class LambdaComparer
    {
        private struct ExpressionPair : IEquatable<ExpressionPair>
        {
            public bool Equals(ExpressionPair other)
            {
                return _left.Equals(other._left) && _right.Equals(other._right);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ExpressionPair) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_left.GetHashCode()*397) ^ _right.GetHashCode();
                }
            }

            public static bool operator ==(ExpressionPair left, ExpressionPair right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ExpressionPair left, ExpressionPair right)
            {
                return !Equals(left, right);
            }

            private readonly Expression _left;
            private readonly Expression _right;

            public ExpressionPair(Expression left, Expression right)
            {
                _left = left;
                _right = right;
            }
        }

        private readonly Dictionary<ExpressionPair, bool> _parsedResult = new Dictionary<ExpressionPair, bool>();

        public bool ContainsOrEqual(Expression left, Expression right)
        {
            if (left == null || right == null)
                return false;
            bool result;
            var expressionPair = new ExpressionPair(left, right);
            if (_parsedResult.TryGetValue(expressionPair, out result))
            {
                return result;
            }
            result = ContainsOrEqualInternal(left, right);
            _parsedResult.Add(expressionPair, result);
            return result;
        }

        protected virtual bool ContainsOrEqualInternal(Expression left, Expression right)
        {
            var rightBinary = right as BinaryExpression;
            var leftBinary = left as BinaryExpression;

            switch (right.NodeType)
            {
                case ExpressionType.AndAlso:
                    if (rightBinary != null)
                    {
                        if (leftBinary == null || left.NodeType != ExpressionType.AndAlso)
                            return false;

                        if (ContainsOrEqual(leftBinary.Left, right) ||
                            ContainsOrEqual(leftBinary.Right, right))
                        {
                            return true;
                        }
                        return BinaryCompareBoth(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.OrElse:
                    if (rightBinary != null)
                    {
                        if (ContainsOrEqual(left, rightBinary.Left) ||
                            ContainsOrEqual(left, rightBinary.Right))
                            return true;
                        if (leftBinary != null)
                        {
                            return BinaryCompareBoth(leftBinary, rightBinary);
                        }
                    }
                    return false;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    if (right.NodeType != left.NodeType)
                    {
                        if (leftBinary != null && left.NodeType == ExpressionType.AndAlso)
                        {
                            return ContainsOrEqual(leftBinary.Left, right) ||
                                   ContainsOrEqual(leftBinary.Right, right);
                        }
                        return false;
                    }
                    return BinaryCompareBoth(leftBinary, rightBinary);
                case ExpressionType.GreaterThan:
                    switch (left.NodeType)
                    {
                        case ExpressionType.GreaterThan:
                            return BinaryCompare(leftBinary, rightBinary);
                        case ExpressionType.LessThan:
                            return BinaryCompareReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.LessThan:
                    switch (left.NodeType)
                    {
                        case ExpressionType.LessThan:
                            return BinaryCompare(leftBinary, rightBinary);
                        case ExpressionType.GreaterThan:
                            return BinaryCompareReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.GreaterThanOrEqual:
                    switch (left.NodeType)
                    {
                        case ExpressionType.GreaterThanOrEqual:
                            return BinaryCompare(leftBinary, rightBinary);
                        case ExpressionType.LessThanOrEqual:
                            return BinaryCompareReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.LessThanOrEqual:
                    switch (left.NodeType)
                    {
                        case ExpressionType.LessThanOrEqual:
                            return BinaryCompare(leftBinary, rightBinary);
                        case ExpressionType.GreaterThanOrEqual:
                            return BinaryCompareReverse(leftBinary, rightBinary);
                    }
                    return false;
                default:
                    if (right.NodeType != left.NodeType)
                        return false;
                    if (rightBinary == null)
                        //Decompose parameters to type name, evaluate constants and collections. Compare resulted set.
                        return EqualsInternal(right, left);
                    if (leftBinary == null)
                        return false;
                    return ContainsOrEqual(rightBinary.Left, leftBinary.Left) &&
                           ContainsOrEqual(rightBinary.Right, leftBinary.Right);
            }
        }

        private static bool EqualsInternal(Expression x, Expression y)
        {
            var xDecomposed = Evaluator.PartialEval(x, e => true);
            xDecomposed = LocalCollectionExpander.Rewrite(xDecomposed);
            string xRepro = xDecomposed.ToString();

            var yDecomposed = Evaluator.PartialEval(y, e => true);
            yDecomposed = LocalCollectionExpander.Rewrite(yDecomposed);
            string yRepro = yDecomposed.ToString();

            return string.Equals(xRepro, yRepro);
        }

        private bool BinaryCompareBoth(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return BinaryCompare(leftBinary, rightBinary) ||
                   BinaryCompareReverse(leftBinary, rightBinary);
        }

        private bool BinaryCompare(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return ContainsOrEqual(leftBinary.Left, rightBinary.Left) &&
                   ContainsOrEqual(leftBinary.Right, rightBinary.Right);
        }

        private bool BinaryCompareReverse(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return ContainsOrEqual(leftBinary.Left, rightBinary.Right) &&
                   ContainsOrEqual(leftBinary.Right, rightBinary.Left);
        }
    }

    public static class LambdaBodyOrPartsComparer
    {
        public static bool ContainsCondition(this LambdaExpression left, LambdaExpression right)
        {
            var comparer = new LambdaComparer();
            return comparer.ContainsOrEqual(left?.Body, right?.Body);
        }
    }
}