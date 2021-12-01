using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraProvider.Connection
{
	public class JiraClient
	{
		private SqlConnectionStringBuilder _builder;

		public JiraClient(string connectionString)
		{
			this._builder = new SqlConnectionStringBuilder(connectionString);
		}

		public object RequestData<TResult>(string query)
		{
			Type targetType = typeof(TResult);

			using (SqlConnection connection = new SqlConnection(this._builder.ConnectionString))
			{
				connection.Open();

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					return this.ExecuteCommand(targetType, command);
				}
			}
		}

		private object ExecuteCommand(Type targetType, SqlCommand command)
		{
			if (targetType.IsGenericType && targetType.IsAssignableTo(typeof(IEnumerable)))
			{
				Type iEnumerableArgumentType = targetType.GenericTypeArguments[0];

				using (SqlDataReader reader = command.ExecuteReader())
				{
					Type enumerableTypeToActivate = typeof(List<>).MakeGenericType(iEnumerableArgumentType);
					object enumerable = Activator.CreateInstance(enumerableTypeToActivate);

					while (reader.Read())
					{
						object instance = Activator.CreateInstance(iEnumerableArgumentType);
						instance = this.MapReaderToInstance(reader, instance);

						MethodInfo addMethodInfo = enumerableTypeToActivate
							.GetMethod("Add", new Type[] { iEnumerableArgumentType });

						addMethodInfo.Invoke(enumerable, new object[] { instance });
					}

					return enumerable;
				}
			}

			return command.ExecuteScalar();
		}

		private object MapReaderToInstance(SqlDataReader reader, object instance)
		{
			var propertieToMap = instance.GetType().GetProperties();

			if (reader.HasRows)
			{
				foreach (PropertyInfo property in propertieToMap)
				{
					object propValue;
					var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

					if (columnAttribute?.Name != null)
					{
						propValue = reader[columnAttribute.Name];
					}
					else
					{
						propValue = reader[property.Name];
					}

					if (propValue.GetType() == property.PropertyType)
					{
						property.SetValue(instance, propValue);
					}
				}
			}

			return instance;
		}
	}
}
