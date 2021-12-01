using System.Linq.Expressions;

namespace JiraProvider.Connection.Visitor
{
	public static class QueryFormatter
	{
		public static Expression GetExpressionIfAnyBinaryOperandtNull(BinaryExpression node)
		{
			if (node.Right is ConstantExpression rightConstExpr && rightConstExpr.Value == null)
			{
				return node.Update(node.Right, node.Conversion, node.Right);
			}
			else if (node.Left is ConstantExpression leftConstExpr && leftConstExpr.Value == null)
			{
				return node.Update(node.Right, node.Conversion, node.Right);

			}

			return null;
		}
	}
}
