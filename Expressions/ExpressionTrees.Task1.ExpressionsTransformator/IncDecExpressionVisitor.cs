using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
	public class IncDecExpressionVisitor : ExpressionVisitor
	{
		public Expression ReduceIncrementDecrementExpressions(Expression node)
		{
			return base.Visit(node);
		}

		/// <summary>
		/// Converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitBinary(BinaryExpression node)
		{
			Expression newExpression = null;
			bool isRightHandConstantNumberOne = node.Right.NodeType == ExpressionType.Constant && this.IsIntegerConstantValue(node.Right, 1);

			switch (node.NodeType)
			{
				case ExpressionType.Add:
					{
						if (isRightHandConstantNumberOne)
						{
							newExpression = Expression.Increment(node.Left);
						}

						break;
					}
				case ExpressionType.Subtract:
					{
						if (isRightHandConstantNumberOne)
						{
							newExpression = Expression.Decrement(node.Left);
						}

						break;
					}
				default:
					break;
			}

			return newExpression ?? node;
		}

		/// <summary>
		/// Helper method.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		private bool IsIntegerConstantValue(Expression node, int valueToCompare)
		{
			if (node is ConstantExpression constant && constant.Value is int value)
			{
				return value.Equals(valueToCompare);
			}

			return false;
		}
	}
}
