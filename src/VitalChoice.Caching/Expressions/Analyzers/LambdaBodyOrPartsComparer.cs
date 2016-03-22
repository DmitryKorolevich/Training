using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class LambdaComparer
    {
        public bool EqualTo(Expression left, Expression right)
        {
            if (left == null || right == null)
                return false;
            return EqualToInternal(left, right);
        }

        public bool ContainsOrEqual(Expression left, Expression right)
        {
            if (left == null || right == null)
                return false;
            return ContainsOrEqualInternal(left, right);
        }

        protected virtual bool EqualToInternal(Expression left, Expression right)
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

                        return EqualBinaryBoth(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.OrElse:
                    if (rightBinary != null)
                    {
                        if (leftBinary != null)
                        {
                            return EqualBinaryBoth(leftBinary, rightBinary);
                        }
                    }
                    return false;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return EqualBinaryBoth(leftBinary, rightBinary);
                case ExpressionType.GreaterThan:
                    switch (left.NodeType)
                    {
                        case ExpressionType.GreaterThan:
                            return EqualBinary(leftBinary, rightBinary);
                        case ExpressionType.LessThan:
                            return EqualBinaryReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.LessThan:
                    switch (left.NodeType)
                    {
                        case ExpressionType.LessThan:
                            return EqualBinary(leftBinary, rightBinary);
                        case ExpressionType.GreaterThan:
                            return EqualBinaryReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.GreaterThanOrEqual:
                    switch (left.NodeType)
                    {
                        case ExpressionType.GreaterThanOrEqual:
                            return EqualBinary(leftBinary, rightBinary);
                        case ExpressionType.LessThanOrEqual:
                            return EqualBinaryReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.LessThanOrEqual:
                    switch (left.NodeType)
                    {
                        case ExpressionType.LessThanOrEqual:
                            return EqualBinary(leftBinary, rightBinary);
                        case ExpressionType.GreaterThanOrEqual:
                            return EqualBinaryReverse(leftBinary, rightBinary);
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
                    return EqualTo(rightBinary.Left, leftBinary.Left) &&
                           EqualTo(rightBinary.Right, leftBinary.Right);
            }
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
                        return ContainsOrEqualBinaryBoth(leftBinary, rightBinary);
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
                            return ContainsOrEqualBinaryBoth(leftBinary, rightBinary);
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
                    return ContainsOrEqualBinaryBoth(leftBinary, rightBinary);
                case ExpressionType.GreaterThan:
                    switch (left.NodeType)
                    {
                        case ExpressionType.GreaterThan:
                            return ContainsOrEqualBinary(leftBinary, rightBinary);
                        case ExpressionType.LessThan:
                            return ContainsOrEqualBinaryReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.LessThan:
                    switch (left.NodeType)
                    {
                        case ExpressionType.LessThan:
                            return ContainsOrEqualBinary(leftBinary, rightBinary);
                        case ExpressionType.GreaterThan:
                            return ContainsOrEqualBinaryReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.GreaterThanOrEqual:
                    switch (left.NodeType)
                    {
                        case ExpressionType.GreaterThanOrEqual:
                            return ContainsOrEqualBinary(leftBinary, rightBinary);
                        case ExpressionType.LessThanOrEqual:
                            return ContainsOrEqualBinaryReverse(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.LessThanOrEqual:
                    switch (left.NodeType)
                    {
                        case ExpressionType.LessThanOrEqual:
                            return ContainsOrEqualBinary(leftBinary, rightBinary);
                        case ExpressionType.GreaterThanOrEqual:
                            return ContainsOrEqualBinaryReverse(leftBinary, rightBinary);
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

        private bool EqualBinaryBoth(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return EqualBinary(leftBinary, rightBinary) ||
                   EqualBinaryReverse(leftBinary, rightBinary);
        }

        private bool EqualBinary(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return EqualTo(leftBinary.Left, rightBinary.Left) &&
                   EqualTo(leftBinary.Right, rightBinary.Right);
        }

        private bool EqualBinaryReverse(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return EqualTo(leftBinary.Left, rightBinary.Right) &&
                   EqualTo(leftBinary.Right, rightBinary.Left);
        }

        private bool ContainsOrEqualBinaryBoth(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return ContainsOrEqualBinary(leftBinary, rightBinary) ||
                   ContainsOrEqualBinaryReverse(leftBinary, rightBinary);
        }

        private bool ContainsOrEqualBinary(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return ContainsOrEqual(leftBinary.Left, rightBinary.Left) &&
                   ContainsOrEqual(leftBinary.Right, rightBinary.Right);
        }

        private bool ContainsOrEqualBinaryReverse(BinaryExpression leftBinary, BinaryExpression rightBinary)
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

        public static bool EqualsToCondition(this LambdaExpression left, LambdaExpression right)
        {
            var comparer = new LambdaComparer();
            return comparer.EqualTo(left?.Body, right?.Body);
        }
    }
}