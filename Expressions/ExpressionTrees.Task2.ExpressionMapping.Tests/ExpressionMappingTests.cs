using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
	[TestClass]
	public class ExpressionMappingTests
	{
		private Foo SourceModel { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			this.SourceModel = new Foo
			{
				IntegerProp = 111,
				StringProp = "string value",
				ObjectProperty = new object()
			};
		}

		[TestMethod]
		public void MapTo_TypeAndName_InitialisedPropertiesWithoutFalsePrefix()
		{
			var mapGenerator = new MappingGenerator();
			var mapper = mapGenerator.Generate<Foo, Bar>();

			Bar destination = mapper.Map(this.SourceModel);

			Assert.AreEqual(this.SourceModel.IntegerProp, destination.IntegerProp);
			Assert.AreEqual(this.SourceModel.StringProp, destination.StringProp);
			Assert.AreEqual(this.SourceModel.ObjectProperty, destination.ObjectProperty);

			Assert.AreEqual(destination.FalseStringProp, default);
			Assert.IsNull(destination.FalseIntegerProp);
		}

		[TestMethod]
		public void MapToTypeAndName_Configuration_InitialisedAllProperties()
		{
			// Assine
			var mapGenerator = new MappingGenerator();
			OptionsBuilder builder = new OptionsBuilder();

			builder.Set<Foo, Bar, string>(
				foo => foo.StringProp,
				bar => bar.FalseIntegerProp
			);

			builder.Set<Foo, Bar, int>(
				foo => foo.IntegerProp,
				bar => bar.FalseStringProp
			);

			mapGenerator.Configure<Foo, Bar>(builder);
			var mapper = mapGenerator.Generate<Foo, Bar>();

			// Act
			Bar destination = mapper.Map(this.SourceModel);

			//Assert
			Assert.AreEqual(this.SourceModel.IntegerProp, destination.IntegerProp);
			Assert.AreEqual(this.SourceModel.StringProp, destination.StringProp);
			Assert.AreEqual(this.SourceModel.ObjectProperty, destination.ObjectProperty);

			Assert.AreEqual(destination.FalseStringProp, this.SourceModel.IntegerProp);
			Assert.AreEqual(destination.FalseIntegerProp, this.SourceModel.StringProp);
		}
	}
}
