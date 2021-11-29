using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JiraProvider.Tests
{
	[TestClass]
	public class DataSetTest
	{
		JiraDataSet<string> _tasks;

		[TestInitialize]
		public void Initialize()
		{
			var initialExpression = Expression.Empty();
			var provider = new Provider("");

			this._tasks = new JiraDataSet<string>(initialExpression, provider);
		}

		[TestMethod]
		public void TestMethod1()
		{
			IQueryable<string> query = null;

			query = this._tasks.AsQueryable()
				.Where(title => title.Contains("Docket"));

			query = from workItem in this._tasks.AsQueryable()
					where workItem.Length > 10
					select workItem;

			foreach (var item in query)
			{
				Assert.IsNotNull(item);
			}
		}
	}
}
