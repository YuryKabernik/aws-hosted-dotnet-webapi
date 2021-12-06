using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
	public class MappingGenerator
	{
		public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
		{
			Expression<Func<TSource, TDestination>> mapFunction =
				this.CreateInstance<TSource, TDestination>();

			return new Mapper<TSource, TDestination>(mapFunction.Compile());
		}

		private Expression<Func<TSource, TDestination>> CreateInstance<TSource, TDestination>()
		{
			ParameterExpression sourceParam = Expression.Parameter(typeof(TSource));
			Expression expression = this.MapProperties<TSource, TDestination>(sourceParam);

			return Expression.Lambda<Func<TSource, TDestination>>(expression, sourceParam);
		}

		private Expression MapProperties<TSource, TDestination>(ParameterExpression sourceParam)
		{
			Type destinationType = typeof(TDestination);

			PropertyInfo[] sourceProperties = sourceParam.Type.GetProperties();
			PropertyInfo[] destinationProperties = destinationType.GetProperties();

			// select properties to authomap
			IEnumerable<PropertyInfo> destinationPropertiesToMap = destinationProperties.Where(
				destinationProperty => sourceProperties.Any(
					sourceProperty =>
						destinationProperty.Name == sourceProperty.Name &&
						destinationProperty.PropertyType == sourceProperty.PropertyType
				)
			);

			// create assignments
			IEnumerable<MemberAssignment> memberAssignments = destinationPropertiesToMap
				.Select(destination =>
				{
					PropertyInfo source = sourceProperties.First(s => s.Name == destination.Name);
					MemberExpression memberValue = Expression.MakeMemberAccess(sourceParam, source);

					return Expression.Bind(destination, memberValue);
				}
			);

			return Expression.MemberInit(Expression.New(destinationType), memberAssignments.ToList());
		}
	}
}
