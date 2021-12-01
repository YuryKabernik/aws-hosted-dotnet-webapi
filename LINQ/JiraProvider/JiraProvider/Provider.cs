using System.Linq;
using System.Linq.Expressions;
using JiraProvider.Connection;

/// <summary>
/// https://docs.microsoft.com/en-us/dotnet/api/system.linq.queryable?view=net-6.0#remarks
/// </summary>
namespace JiraProvider
{
	public class Provider : IQueryProvider
	{
		private JiraClient _client;
		private JiraExpressionVisitor _visitor;

		public Provider(string connectionString)
		{
			this._client = new JiraClient(connectionString);
			this._visitor = new JiraExpressionVisitor();
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return this.CreateQuery<object>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new JiraDataSet<TElement>(expression, this);
		}

		public object Execute(Expression expression)
		{
			return this.Execute<object>(expression);
		}

		public TResult Execute<TResult>(Expression expression) // #2 translates and executes expression tree
		{
			string query = this._visitor.TranslateToSql(expression);
			object requestResult = this._client.RequestData<TResult>(query);

			return (TResult)requestResult;
		}
	}
}
