using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JiraProvider.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JiraProvider.Tests
{
	[TestClass]
	public class DataSetTest
	{
		JiraDataSet<WorkItem> _tasks;
		JiraDataSet<TeamMember> _teamMembers;

		[TestInitialize]
		public void Initialize()
		{
			var provider = new Provider("Data Source=localhost;Initial Catalog=ExpressionDB;Integrated Security=True;Pooling=False");

			this._tasks = new JiraDataSet<WorkItem>(null, provider);
			this._teamMembers = new JiraDataSet<TeamMember>(null, provider);
		}

		[TestMethod]
		public void TestGetWorkItems_TitleMoreThan10Chars_ReturnsTwoItems()
		{
			IQueryable<WorkItem> query = null;

			query = from workItem in this._tasks
					where workItem.Title.Length > 10
					select workItem;

			foreach (WorkItem item in query)
			{
				Assert.IsNotNull(item);
				this.DebugPrintWorkItem(item);
			}
		}

		[TestMethod]
		public void TestGetWorkItems_EqualityExpression_ReturnsSingleItem()
		{
			IQueryable<WorkItem> query = null;

			query = from workItem in this._tasks
					where workItem.Title == "My Content"
					select workItem;

			foreach (var item in query)
			{
				Assert.IsNotNull(item);
				this.DebugPrintWorkItem(item);
			}
		}

		[TestMethod]
		public void TestGetWorkItems_PriorityLessThanSix_ReturnsTwoItems()
		{
			IQueryable<WorkItem> query = null;

			query = from workItem in this._tasks
					where workItem.Description != null
					select workItem;

			foreach (var item in query)
			{
				Assert.IsNotNull(item);
				this.DebugPrintWorkItem(item);
			}
		}

		[TestMethod]
		public void TestGetTeamMembers_TitleMoreThan10Chars_ReturnsManyItems()
		{
			IQueryable<TeamMember> query = null;

			query = from member in this._teamMembers
					where member.Age > 30
					select member;

			foreach (TeamMember item in query)
			{
				Assert.IsNotNull(item);
				this.DebugPrintTeamMembers(item);
			}
		}

		[TestMethod]
		public void TestGetTeamMembers_EqualityExpression_ReturnsSingleItem()
		{
			IQueryable<TeamMember> query = null;

			query = from member in this._teamMembers
					where member.Title == "Sofrware Developer"
					select member;

			foreach (TeamMember item in query)
			{
				Assert.IsNotNull(item);
				this.DebugPrintTeamMembers(item);
			}
		}

		[TestMethod]
		public void TestGetTeamMembers_AgeLessThan30_ReturnsSingleItem()
		{
			IQueryable<TeamMember> query = null;

			query = from member in this._teamMembers
					where member.Age < 30
					select member;

			foreach (TeamMember item in query)
			{
				Assert.IsNotNull(item);
				this.DebugPrintTeamMembers(item);
			}
		}

		[TestMethod]
		public void MethodBasedQuery_ComplexWhereStatement_ReturnsSingleItem()
		{
			List<TeamMember> teamMembers = this._teamMembers
				.Where(member => member.Age > 30 && member.LastName.Length == 3)
				.ToList();

			foreach (TeamMember item in teamMembers)
			{
				Assert.IsNotNull(item);
				this.DebugPrintTeamMembers(item);
			}
		}

		private void DebugPrintWorkItem(WorkItem item)
		{
			Debug.WriteLine("-----------");
			Debug.WriteLine(item.Title);
			Debug.WriteLine(item.Priority);
			Debug.WriteLine(item.Description ?? "NULL");
		}

		private void DebugPrintTeamMembers(TeamMember item)
		{
			Debug.WriteLine("-----------");
			Debug.WriteLine(item.Name);
			Debug.WriteLine(item.LastName);
			Debug.WriteLine(item.Title);
			Debug.WriteLine(item.Age);
		}
	}
}
