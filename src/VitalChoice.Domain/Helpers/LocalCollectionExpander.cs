using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VitalChoice.Domain.Helpers
{
    internal struct ItemPair<T1, T2>
    {
        public ItemPair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
    }

    /// <summary>
    /// Enables cache key support for local collection values.
    /// </summary>
    internal class LocalCollectionExpander : ExpressionVisitor
    {
        public static Expression Rewrite(Expression expression)
        {
            return new LocalCollectionExpander().Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var map = node.Method.GetParameters()
                .Zip(node.Arguments, (p, a) => new ItemPair<Type, Expression>(p.ParameterType, a))
                .ToLinkedList();

            var instanceType = node.Object?.Type;
            map.AddFirst(new ItemPair<Type, Expression>(instanceType, node.Object));

            // for any local collection parameters in the method, make a
            // replacement argument which will print its elements
            var replacements = (from x in map
                where x.Value1 != null && x.Value1.IsImplementGeneric(typeof(IEnumerable<>))
                where x.Value2.NodeType == ExpressionType.Constant
                let elementType = x.Value1.TryGetElementType(typeof(IEnumerable<>))
                let printer = MakePrinter((ConstantExpression)x.Value2, elementType)
                select new ItemPair<Expression, ConstantExpression>(x.Value2, printer)).ToArray();

            if (replacements.Any())
            {
                var args =
                    map.Select(x => replacements.Where(r => x.Value2 == r.Value1).Select(r => r.Value2).SingleOrDefault() ?? x.Value2)
                        .ToArray();

                node = node.Update(args.First(), args.Skip(1));
            }

            return base.VisitMethodCall(node);
        }

        ConstantExpression MakePrinter(ConstantExpression enumerable, Type elementType)
        {
            var value = (IEnumerable)enumerable.Value;
            var printerType = typeof(Printer<>).MakeGenericType(elementType);
            var printer = Activator.CreateInstance(printerType, value);

            return Expression.Constant(printer);
        }

        /// <summary>
        /// Overrides ToString to print each element of a collection.
        /// </summary>
        /// <remarks>
        /// Inherits List in order to support List.Contains instance method as well
        /// as standard Enumerable.Contains/Any extension methods.
        /// </remarks>
        class Printer<T> : List<T>
        {
            public Printer(IEnumerable collection)
            {
                AddRange(collection.Cast<T>());
            }

            public override string ToString()
            {
                return $"{{{this.ToConcatenatedString(t => t.ToString(), "|")}}}";
            }
        }
    }
}