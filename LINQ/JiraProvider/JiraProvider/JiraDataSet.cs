using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JiraProvider
{
	public class JiraDataSet<T> : IQueryable<T>
    {
        protected readonly Expression _expression;
        protected readonly IQueryProvider _queryProvider;

		public JiraDataSet(Expression expression, IQueryProvider provider = null)
		{
            this._expression = expression ?? Expression.Constant(this);
            this._queryProvider = provider ?? new Provider("");
		}

        public Type ElementType => typeof(T);

		public Expression Expression => this._expression; // Expression Tree, that is building up by extention methods

        public IQueryProvider Provider => this._queryProvider; // provider is responsible for creating a new Query and Translating expression tree to an actual query

        public IEnumerator<T> GetEnumerator()
        {
            return this._queryProvider.Execute<IEnumerable<T>>(this._expression).GetEnumerator(); // #1 query execution on foreach like enumeration operation
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._queryProvider.Execute<IEnumerable>(this._expression).GetEnumerator(); // #1 query execution on foreach like enumeration operation
        }
    }
}
