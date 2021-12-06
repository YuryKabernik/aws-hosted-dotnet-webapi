using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
	public class OptionsBuilder
	{
		/// <summary>
		/// Key -> source
		/// Value -> destination
		/// </summary>
		public IDictionary<MemberInfo, MemberInfo> Expressions { get; }

		public OptionsBuilder()
		{
			this.Expressions = new Dictionary<MemberInfo, MemberInfo>();
		}

		/// <summary>
		/// Selects source member.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="sourceSelector"></param>
		/// <returns></returns>
		public void Set<TSource, TDestination, TProperty>(
			Expression<Func<TSource, TProperty>> sourceSelector,
			Expression<Func<TDestination, TProperty>> destinationSelector
		)
		{
			if (
				sourceSelector.Body.NodeType == ExpressionType.MemberAccess &&
				destinationSelector.Body.NodeType == ExpressionType.MemberAccess)
			{
				if (destinationSelector.ReturnType != sourceSelector.ReturnType)
				{
					throw new ArgumentException($"Invalid return type! Expected: {destinationSelector.ReturnType} \t Actual: {sourceSelector.ReturnType}");
				}

				MemberExpression sourceMember = (MemberExpression)sourceSelector.Body;
				MemberExpression destinationMember = (MemberExpression)destinationSelector.Body;

				this.Expressions.Add(sourceMember.Member, destinationMember.Member);
			}
		}
	}
}
