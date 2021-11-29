using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// https://docs.microsoft.com/en-us/dotnet/api/system.linq.queryable?view=net-6.0#remarks
/// </summary>
namespace JiraProvider
{
	public class Provider : IQueryProvider
	{
		private SqlConnectionStringBuilder _builder;

		public Provider(string connectionString)
		{
			this._builder = new SqlConnectionStringBuilder(connectionString);
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
			string query = this.TranslateToSql(expression);
			object requestResult = this.RequestData<TResult>(query);

			return (TResult)requestResult;
		}

		private string TranslateToSql(Expression expression)
		{
			return string.Empty;
		}

		private TResult RequestData<TResult>(string query)
		{
			bool isResultEnumerable = typeof(TResult).IsEnum;
			TResult result = default(TResult);

			using (SqlConnection connection = new SqlConnection(this._builder.ConnectionString))
			{
				connection.Open();

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							if (isResultEnumerable)
							{
								
							} else
							{

							}
						}
					}
				}
			}

			return result;
		}
	}
}
