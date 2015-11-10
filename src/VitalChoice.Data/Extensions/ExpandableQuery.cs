using Microsoft.Data.Entity.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Query.Internal;

namespace VitalChoice.Data.Extensions
{
//	/// <summary>
//	/// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
//	/// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
//	/// </summary>
//	public class ExpandableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable, IAsyncEnumerable<T>
//    {
//	    readonly ExpandableQueryProvider<T> _provider;
//	    readonly IQueryable<T> _inner;

//		internal IQueryable<T> InnerQuery => _inner;
//// Original query, that we're wrapping

//		internal ExpandableQuery (IQueryable<T> inner)
//		{
//			_inner = inner;
//			_provider = new ExpandableQueryProvider<T> (this);
//		}

//		Expression IQueryable.Expression => _inner.Expression;
//	    Type IQueryable.ElementType => typeof (T);
//	    IQueryProvider IQueryable.Provider => _provider;
//	    public IEnumerator<T> GetEnumerator () => _inner.GetEnumerator ();
//	    IEnumerator IEnumerable.GetEnumerator () => _inner.GetEnumerator ();
//	    public override string ToString () => _inner.ToString ();

//	    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
//        {
//            var asyncEnumerable = _inner as IAsyncEnumerable<T>;
//            if (asyncEnumerable != null)
//                return asyncEnumerable.GetEnumerator();
//            return new ExpandableDbAsyncEnumerator<T>(_inner.GetEnumerator());
//        }
//    }

//	class ExpandableQueryProvider<T> : IQueryProvider, IAsyncQueryProvider
//    {
//	    readonly ExpandableQuery<T> _query;

//		internal ExpandableQueryProvider (ExpandableQuery<T> query)
//		{
//			_query = query;
//		}

//		// The following four methods first call ExpressionExpander to visit the expression tree, then call
//		// upon the inner query to do the remaining work.

//		IQueryable<TElement> IQueryProvider.CreateQuery<TElement> (Expression expression)
//		{
//			return new ExpandableQuery<TElement> (_query.InnerQuery.Provider.CreateQuery<TElement> (expression.Expand()));
//		}

//		IQueryable IQueryProvider.CreateQuery (Expression expression)
//		{
//			return _query.InnerQuery.Provider.CreateQuery (expression.Expand());
//		}

//		TResult IQueryProvider.Execute<TResult> (Expression expression)
//		{
//			return _query.InnerQuery.Provider.Execute<TResult> (expression.Expand());
//		}

//		object IQueryProvider.Execute (Expression expression)
//		{
//			return _query.InnerQuery.Provider.Execute (expression.Expand());
//		}

//        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
//        {
//            var asyncProvider = _query.InnerQuery.Provider as IAsyncQueryProvider;
//            return asyncProvider?.ExecuteAsync<TResult>(expression.Expand());
//        }

//        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
//        {
//            var asyncProvider = _query.InnerQuery.Provider as IAsyncQueryProvider;
//            if (asyncProvider != null)
//                return asyncProvider.ExecuteAsync<TResult>(expression.Expand(), cancellationToken);
//            return Task.FromResult(_query.InnerQuery.Provider.Execute<TResult>(expression.Expand()));
//        }
//    }
}
