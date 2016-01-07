using System;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public static class LambdaBodyOrPartsComparer
    {
        public static bool ContainsCondition(this LambdaExpression left, LambdaExpression right)
        {
            return left.Body.ContainsOrEqual(right?.Body);
        }

        private static bool ContainsOrEqual(this Expression left, Expression right)
        {
            if (left == null || right == null)
                return false;

            var rightBinary = right as BinaryExpression;
            var leftBinary = left as BinaryExpression;

            switch (right.NodeType)
            {
                case ExpressionType.AndAlso:
                    if (rightBinary != null)
                    {
                        if (leftBinary == null || left.NodeType != ExpressionType.AndAlso)
                            return false;

                        if (leftBinary.Left.ContainsOrEqual(right) ||
                            leftBinary.Right.ContainsOrEqual(right))
                        {
                            return true;
                        }
                        return BinaryCompareBoth(leftBinary, rightBinary);
                    }
                    return false;
                case ExpressionType.OrElse:
                    if (rightBinary != null)
                    {
                        if (left.ContainsOrEqual(rightBinary.Left) ||
                            left.ContainsOrEqual(rightBinary.Right))
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
                            return leftBinary.Left.ContainsOrEqual(right) ||
                                   leftBinary.Right.ContainsOrEqual(right);
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
                        return EqualsInternal(right, left);
                    if (leftBinary == null)
                        return false;
                    return rightBinary.Left.ContainsOrEqual(leftBinary.Left) &&
                           rightBinary.Right.ContainsOrEqual(leftBinary.Right);
                //Decompose parameters to type name, evaluate constants and collections. Compare resulted set.
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

        private static bool BinaryCompareBoth(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return leftBinary.Left.ContainsOrEqual(rightBinary.Left) &&
                   leftBinary.Right.ContainsOrEqual(rightBinary.Right) ||
                   leftBinary.Left.ContainsOrEqual(rightBinary.Right) &&
                   leftBinary.Right.ContainsOrEqual(rightBinary.Left);
        }

        private static bool BinaryCompare(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return leftBinary.Left.ContainsOrEqual(rightBinary.Left) &&
                   leftBinary.Right.ContainsOrEqual(rightBinary.Right);
        }

        private static bool BinaryCompareReverse(BinaryExpression leftBinary, BinaryExpression rightBinary)
        {
            if (leftBinary == null)
                return false;
            if (rightBinary == null)
                return false;
            return leftBinary.Left.ContainsOrEqual(rightBinary.Right) &&
                   leftBinary.Right.ContainsOrEqual(rightBinary.Left);
        }
    }
}