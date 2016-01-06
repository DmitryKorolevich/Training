using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class LambdaBodyOrPartsComparer
    {
        public bool GreaterOrEqualsTo(Expression left, Expression right)
        {
            BinaryExpression lBinary = left as BinaryExpression;
            BinaryExpression rBinary = right as BinaryExpression;

            switch (left.NodeType)
            {
                case ExpressionType.AndAlso:
                    if (lBinary != null)
                    {
                        if (rBinary != null)
                        {
                            return GreaterOrEqualsTo(lBinary.Left, rBinary.Left) && GreaterOrEqualsTo(lBinary.Right, rBinary.Right)
                        }
                        return false;
                        //return GreaterOrEqualsTo(lBinary.Left, right) || GreaterOrEqualsTo(lBinary.Right, right);
                    }
                case ExpressionType.OrElse:
                    if (lBinary != null)
                    {
                        if (rBinary != null)
                        {
                            
                        }
                        return GreaterOrEqualsTo(lBinary.Left, right) || GreaterOrEqualsTo(lBinary.Right, right);
                    }
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    if (left.NodeType != right.NodeType)
                        return false;
                    return BinaryCompareBoth(lBinary, rBinary);
                case ExpressionType.GreaterThan:
                    if (right.NodeType == ExpressionType.GreaterThan)
                    {
                        return BinaryCompare(lBinary, rBinary);
                    }
                    if (right.NodeType == ExpressionType.LessThan)
                    {
                        return BinaryCompareReverse(lBinary, rBinary);
                    }
                    return false;
                case ExpressionType.LessThan:
                    if (right.NodeType == ExpressionType.LessThan)
                    {
                        return BinaryCompare(lBinary, rBinary);
                    }
                    if (right.NodeType == ExpressionType.GreaterThan)
                    {
                        return BinaryCompareReverse(lBinary, rBinary);
                    }
                    return false;
                case ExpressionType.GreaterThanOrEqual:
                    if (right.NodeType == ExpressionType.GreaterThanOrEqual)
                    {
                        return BinaryCompare(lBinary, rBinary);
                    }
                    if (right.NodeType == ExpressionType.LessThanOrEqual)
                    {
                        return BinaryCompareReverse(lBinary, rBinary);
                    }
                    return false;
                case ExpressionType.LessThanOrEqual:
                    if (right.NodeType == ExpressionType.LessThanOrEqual)
                    {
                        return BinaryCompare(lBinary, rBinary);
                    }
                    if (right.NodeType == ExpressionType.GreaterThanOrEqual)
                    {
                        return BinaryCompareReverse(lBinary, rBinary);
                    }
                    return false;
                default:
                    if (left.NodeType != right.NodeType)
                        return false;
                    if (lBinary != null)
                    {
                        if (rBinary == null)
                            return false;
                        return GreaterOrEqualsTo(lBinary.Left, rBinary.Left) && Equals(lBinary.Right, rBinary.Right);
                    }
                    //Decompose parameters to type name, evaluate constants and collections. Compare resulted set.
                    return EqualsInternal(left, right);
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

        private bool BinaryCompareLessThanOrEquals(BinaryExpression xBinary, BinaryExpression yBinary)
        {
            if (xBinary != null)
            {
                if (yBinary == null)
                    return false;
                return Equals(xBinary.Left, yBinary.Left) && Equals(xBinary.Right, yBinary.Right) ||
                       Equals(xBinary.Left, yBinary.Right) && Equals(xBinary.Right, yBinary.Left);
            }
            return false;
        }

        private bool BinaryCompareBoth(BinaryExpression xBinary, BinaryExpression yBinary)
        {
            if (xBinary != null)
            {
                if (yBinary == null)
                    return false;
                return Equals(xBinary.Left, yBinary.Left) && Equals(xBinary.Right, yBinary.Right) ||
                       Equals(xBinary.Left, yBinary.Right) && Equals(xBinary.Right, yBinary.Left);
            }
            return false;
        }

        private bool BinaryCompare(BinaryExpression xBinary, BinaryExpression yBinary)
        {
            if (xBinary != null)
            {
                if (yBinary == null)
                    return false;
                return Equals(xBinary.Left, yBinary.Left) && Equals(xBinary.Right, yBinary.Right);
            }
            return false;
        }

        private bool BinaryCompareReverse(BinaryExpression xBinary, BinaryExpression yBinary)
        {
            if (xBinary != null)
            {
                if (yBinary == null)
                    return false;
                return Equals(xBinary.Left, yBinary.Right) && Equals(xBinary.Right, yBinary.Left);
            }
            return false;
        }

        public int GetHashCode(Expression obj)
        {
            return obj.GetHashCode();
        }
    }
}