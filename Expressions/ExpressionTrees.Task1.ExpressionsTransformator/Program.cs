/*
 * Create a class based on ExpressionVisitor, which makes expression tree transformation:
 * 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
 * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
 *    - source expression;
 *    - dictionary: <parameter name: value for replacement>
 * The results could be printed in console or checked via Debugger using any Visualizer.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Expression Visitor for increment/decrement.");
			Console.WriteLine();

			ConvertIncremention();

			Console.WriteLine("\n// ------- //");

			ChangeParameterValues();

			Console.ReadLine();
		}

		/// <summary>
		/// 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
		/// </summary>
		/// <param name="visitor"></param>
		static void ConvertIncremention()
		{
			int param = 3;
			var visitor = new IncDecExpressionVisitor();

			Expression<Func<int, int>> expressionAddOne = valueA => valueA + 1;
			var updatedAddOne = (Expression<Func<int, int>>)
				visitor.ReduceIncrementDecrementExpressions(expressionAddOne);

			int resultOfAddOne = (expressionAddOne.Compile())(param);
			int resultOfUpdatedAddOne = (updatedAddOne.Compile())(param);

			Console.Write($"\nOriginal AddOne execution result: {resultOfAddOne}\t");
			Console.WriteLine($"Old expression: {expressionAddOne}");

			Console.Write($"\nUpdated AddOne execution result: {resultOfUpdatedAddOne}\t");
			Console.WriteLine($"New expression: {updatedAddOne}");

			Expression<Func<int, int>> expressionDecrementOne = valueA => valueA - 1;
			var updatedDecrementOne = (Expression<Func<int, int>>)
				visitor.ReduceIncrementDecrementExpressions(expressionDecrementOne);

			int resultOfDecrementOne = (expressionDecrementOne.Compile())(param);
			int resultOfUpdatedDecrementOne = (updatedDecrementOne.Compile())(param);

			Console.Write($"\nOriginal DecrementOne execution result: {resultOfDecrementOne}\t");
			Console.WriteLine($"Old expression: {expressionDecrementOne}");

			Console.Write($"\nUpdated DecrementOne execution result: {resultOfUpdatedDecrementOne}\t");
			Console.WriteLine($"New expression: {updatedDecrementOne}");
		}

		/// <summary>
		/// 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
		/// - source expression;
		/// - dictionary: { parameter name: value for replacement }
		/// </summary>
		/// <param name="visitor"></param>
		static void ChangeParameterValues()
		{
			ConfigurableExpressionVisitor visitor = new ConfigurableExpressionVisitor();
			Dictionary<string, object> config = new Dictionary<string, object>();

			config.Add("paramA", 40);
			config.Add("paramB", false);

			Console.WriteLine($"Transformation Params:");
			foreach (var item in config)
			{
				Console.WriteLine($"\t{item.Key}: {item.Value}");
			}

			Expression<Func<int, bool, int>> expressionAddOne = (paramA, paramB) => paramB ? paramA + 5 : paramA - 5;
			Expression<Func<int, bool, int>> transformedExpression = (Expression<Func<int, bool, int>>)
				visitor.ReplaceParamsByConfiguration(expressionAddOne, config);

			int resultValue = (expressionAddOne.Compile())(5, true);

			Console.WriteLine($"\nOld expression:{expressionAddOne}");
			Console.WriteLine($"Result:{resultValue}");

			resultValue = (transformedExpression.Compile())(5, true);

			Console.WriteLine($"\nNew expression:{transformedExpression}");
			Console.WriteLine($"Result:{resultValue}");
		}
	}
}
