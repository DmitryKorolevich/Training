﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    public class ConditionalRemovingExpressionVisitor : ExpressionVisitorBase
    {
        public override Expression Visit(Expression node)
        {
            var selectExpression = node as SelectExpression;

            if (selectExpression != null
                && selectExpression.Projection.Count == 1)
            {
                var conditionalExpression = selectExpression.Projection.First() as ConditionalExpression;

                if (conditionalExpression?.Type == typeof(bool)
                    && conditionalExpression.IfTrue.NodeType == ExpressionType.Constant
                    && conditionalExpression.IfFalse.NodeType == ExpressionType.Constant
                    && (bool)((ConstantExpression)conditionalExpression.IfTrue).Value
                    && !(bool)((ConstantExpression)conditionalExpression.IfFalse).Value)
                {
                    return Visit(conditionalExpression.Test);
                }
            }
            return base.Visit(node);
        }
    }
}
