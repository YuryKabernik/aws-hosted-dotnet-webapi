using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
	public class MappingGenerator
	{
		private IDictionary<MemberInfo, MemberInfo> ConfiguredMemberAssignments { get; set; }

		public MappingGenerator()
		{
			this.ConfiguredMemberAssignments = new Dictionary<MemberInfo, MemberInfo>();
		}

		public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
		{
			Expression<Func<TSource, TDestination>> mapFunction =
				this.CreateInstance<TSource, TDestination>();

			return new Mapper<TSource, TDestination>(mapFunction.Compile());
		}

		public void Configure<TSource, TDestination>(OptionsBuilder builder)
		{
			if (builder.Expressions.Count > 0)
			{
				this.ConfiguredMemberAssignments = builder.Expressions;
			}
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

			// create assignments
			IEnumerable<MemberAssignment> memberAssignments = this.GetMemberAssignments(destinationType, sourceParam);

			if (this.ConfiguredMemberAssignments.Count > 0)
			{
				IEnumerable<MemberAssignment> configuredMemberAssignments = this.ConfiguredMemberAssignments.Select(cfg =>
				{
					MemberExpression memberValue = Expression.MakeMemberAccess(sourceParam, cfg.Key);

					return Expression.Bind(cfg.Value, memberValue);
				});

				memberAssignments = memberAssignments.Concat(configuredMemberAssignments);
			}

			return Expression.MemberInit(Expression.New(destinationType), memberAssignments.ToList());
		}

		private IEnumerable<MemberAssignment> GetMemberAssignments(Type destinationType, ParameterExpression sourceParam)
		{
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

			return destinationPropertiesToMap
				.Select(destination =>
				{
					PropertyInfo source = sourceProperties.First(s => s.Name == destination.Name);
					MemberExpression memberValue = Expression.MakeMemberAccess(sourceParam, source);

					return Expression.Bind(destination, memberValue);
				}
			);
		}
	}
}
