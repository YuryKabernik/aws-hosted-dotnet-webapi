using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
	public class ConfigurableExpressionVisitor : ExpressionVisitor
	{
		private IDictionary<string, object> ParameterReplacementConfig { get; set; }

		/// <summary>
		///  * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
		///  *    - source expression;
		///  *    - dictionary: 
		///				parameter name: value for replacement
		/// </summary>
		/// <param name="node"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public Expression ReplaceParamsByConfiguration(Expression node, IDictionary<string, object> config)
		{
			this.ParameterReplacementConfig = config;

			return this.Visit(node);
		}

		protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node)
		{
			bool requiresTransformation = node.Parameters.Select(p => p.Name).Any(this.ParameterReplacementConfig.ContainsKey);

			if (requiresTransformation)
			{
				// substitude of an original param with constant value
				Expression[] arguments = node.Parameters.Select(parameter =>
					this.ParameterReplacementConfig.TryGetValue(parameter.Name, out object newValue) ?
					Expression.Constant(newValue, newValue.GetType()) :
					(Expression)parameter
				).ToArray();

				Type[] typeArguments = arguments.Select(arg => arg.Type).ToArray();

				MethodInfo methodInfo = node.Type.GetMethod("Invoke");
				
				// create MethosdCall expression of Delegate's Invoke method
				MethodCallExpression newLambdaExpression = Expression.Call(node, methodInfo, arguments);

				// creating a new Labmbda expression of Invoke method call
				return Expression.Lambda<TDelegate>(newLambdaExpression, node.Parameters);
			}

			return node;
		}
	}
}
