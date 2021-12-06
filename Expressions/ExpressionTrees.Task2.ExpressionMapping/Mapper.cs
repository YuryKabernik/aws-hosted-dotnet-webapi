using System;

namespace ExpressionTrees.Task2.ExpressionMapping
{
	public class Mapper<TSource, TDestination>
	{
		private readonly Func<TSource, TDestination> _mapFunction;

		internal Mapper(Func<TSource, TDestination> func)
		{
			this._mapFunction = func;
		}

		public TDestination Map(TSource source)
		{
			return this._mapFunction(source);
		}
	}
}
