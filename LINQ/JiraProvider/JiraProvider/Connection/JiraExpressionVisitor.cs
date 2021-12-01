using System;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using JiraProvider.Connection.Visitor;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraProvider.Connection
{
	/// <summary>
	/// 1.	Операторов:
	///     a.	Select … From … Where
	///     b.	>, <, =
	///     c.AND
	/// 2.	Типы данных:
	///     a.	Numeric
	///     b.	String
	/// </summary>
	internal class JiraExpressionVisitor : ExpressionVisitor
	{
		private StringBuilder _resultStringBuilder = new StringBuilder();

		public string TranslateToSql(Expression expression)
		{
			this.Visit(expression);

			return this._resultStringBuilder.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			bool isQueryableDeclaringType = node.Method.DeclaringType == typeof(Queryable);

			if (!isQueryableDeclaringType)
			{
				return base.VisitMethodCall(node);
			}

			switch (node.Method.Name)
			{
				case "Where":
					{
						this._resultStringBuilder.Append(" WHERE ");
						break;
					}
				case "Select":
					{
						this._resultStringBuilder.Replace(" SELECT *", " SELECT *");
						break;
					}
				default:
					throw new NotSupportedException($"Operation '{node.NodeType}' with method {node.Method.Name} is not supported");
			}

			return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Equal:
					{
						Expression convertedExpression = QueryFormatter.GetExpressionIfAnyBinaryOperandtNull(node);

						if (convertedExpression != null)
						{
							this.Visit(node.Left);
							this._resultStringBuilder.Append(" IS ");
							this.Visit(node.Right);
							break;
						}

						this.Visit(node.Left);
						this._resultStringBuilder.Append(" = ");
						this.Visit(node.Right);

						break;
					}
				case ExpressionType.GreaterThan:
					this.Visit(node.Left);
					this._resultStringBuilder.Append(" > ");
					this.Visit(node.Right);

					break;
				case ExpressionType.LessThan:
					this.Visit(node.Left);
					this._resultStringBuilder.Append(" < ");
					this.Visit(node.Right);

					break;
				case ExpressionType.AndAlso:
					this.Visit(node.Left);
					this._resultStringBuilder.Append(" AND ");
					this.Visit(node.Right);

					break;
				case ExpressionType.NotEqual:
					{
						Expression convertedExpression = QueryFormatter.GetExpressionIfAnyBinaryOperandtNull(node);

						if (convertedExpression != null)
						{
							this.Visit(node.Left);
							this._resultStringBuilder.Append(" IS NOT ");
							this.Visit(node.Right);
							break;
						}

						this._resultStringBuilder.Append(" NOT ");
						this.Visit(node.Left);
						this._resultStringBuilder.Append(" = ");
						this.Visit(node.Right);

						break;
					}
				default:
					throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
			};

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (
				node.Member.MemberType == MemberTypes.Property &&
				node.Member.DeclaringType == typeof(string) &&
				node.Member.Name == "Length")
			{
				this._resultStringBuilder.Append("LEN(");
				base.VisitMember(node);
				this._resultStringBuilder.Append(")");
			}
			else if (
				node.Member.MemberType == MemberTypes.Property &&
				node.Member.ReflectedType.IsClass)
			{
				var columnAttribute = node.Member.GetCustomAttribute<ColumnAttribute>();
				if (columnAttribute?.Name != null)
				{
					this._resultStringBuilder.Append($"[{columnAttribute.Name}]");
				}
				else
				{
					this._resultStringBuilder.Append(node.Member.Name);
				}

				return base.VisitMember(node);
			}

			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node.Value == null)
			{
				this._resultStringBuilder.Append("NULL");
				return base.VisitConstant(node);
			}

			switch (node.Type.Name)
			{
				case nameof(Int32):
					{
						this._resultStringBuilder.Append(node.Value);
						break;
					}
				case nameof(String):
					{
						this._resultStringBuilder.Append($"'{node.Value}'");
						break;
					}
				case "null":
					{
						this._resultStringBuilder.Append("NULL");
						break;
					}
				case "JiraDataSet`1":
					{
						Type dataSetArg = node.Type.GetGenericArguments().First();
						string selectAllFrom = $"SELECT * FROM [dbo].[{dataSetArg.Name}s]";

						this._resultStringBuilder.Insert(0, selectAllFrom);
						break;
					}
				default:
					throw new NotSupportedException($"Operation '{node.Type.Name}' is not supported");
			}

			return base.VisitConstant(node);
		}
	}
}
