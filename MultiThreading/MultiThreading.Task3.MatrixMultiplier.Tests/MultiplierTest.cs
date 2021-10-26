using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiThreading.Task3.MatrixMultiplier.Matrices;
using MultiThreading.Task3.MatrixMultiplier.Multipliers;

namespace MultiThreading.Task3.MatrixMultiplier.Tests
{
	[TestClass]
	public class MultiplierTest
	{
		[TestMethod]
		public void MultiplyMatrix3On3Test()
		{
			this.TestMatrix3On3(new MatricesMultiplier());
			this.TestMatrix3On3(new MatricesMultiplierParallel());
		}

		/// <summary>
		/// Check the size of the matrix which makes parallel multiplication
		/// more effective than the regular one.
		/// </summary>
		[TestMethod]
		public void ParallelEfficiencyTest()
		{
			const byte stepSize = 5;
			byte matrixSize = 50;
			bool isEffectiveSize = false;

			long regTimeStamp = this.TestParallelEfficiency(new MatricesMultiplier(), matrixSize);
			long parallelTimeStamp = this.TestParallelEfficiency(new MatricesMultiplierParallel(), matrixSize);

			while (!isEffectiveSize && matrixSize != byte.MaxValue)
			{
				matrixSize += stepSize;

				regTimeStamp = this.TestParallelEfficiency(new MatricesMultiplier(), matrixSize);
				parallelTimeStamp = this.TestParallelEfficiency(new MatricesMultiplierParallel(), matrixSize);

				isEffectiveSize = regTimeStamp > parallelTimeStamp;
			}

			//
			// Look at the Test Detail Summary to see the effective matrix size.
			//
			Console.WriteLine($"The effective size starts from {matrixSize}");

			Assert.IsTrue(
				regTimeStamp > parallelTimeStamp,
				$"Effectiveness not determined. Regular: {regTimeStamp} vs Parallel: {parallelTimeStamp}"
			);
		}

		#region private methods

		long TestParallelEfficiency(IMatricesMultiplier matrixMultiplier, byte matrixSize)
		{
			var m1 = new Matrix(matrixSize, matrixSize, true);
			var m2 = new Matrix(matrixSize, matrixSize, true);

			Stopwatch stopwatch = Stopwatch.StartNew();

			matrixMultiplier.Multiply(m1, m2);

			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		void TestMatrix3On3(IMatricesMultiplier matrixMultiplier)
		{
			if (matrixMultiplier == null)
			{
				throw new ArgumentNullException(nameof(matrixMultiplier));
			}

			var m1 = new Matrix(3, 3);
			m1.SetElement(0, 0, 34);
			m1.SetElement(0, 1, 2);
			m1.SetElement(0, 2, 6);

			m1.SetElement(1, 0, 5);
			m1.SetElement(1, 1, 4);
			m1.SetElement(1, 2, 54);

			m1.SetElement(2, 0, 2);
			m1.SetElement(2, 1, 9);
			m1.SetElement(2, 2, 8);

			var m2 = new Matrix(3, 3);
			m2.SetElement(0, 0, 12);
			m2.SetElement(0, 1, 52);
			m2.SetElement(0, 2, 85);

			m2.SetElement(1, 0, 5);
			m2.SetElement(1, 1, 5);
			m2.SetElement(1, 2, 54);

			m2.SetElement(2, 0, 5);
			m2.SetElement(2, 1, 8);
			m2.SetElement(2, 2, 9);

			var multiplied = matrixMultiplier.Multiply(m1, m2);
			Assert.AreEqual(448, multiplied.GetElement(0, 0));
			Assert.AreEqual(1826, multiplied.GetElement(0, 1));
			Assert.AreEqual(3052, multiplied.GetElement(0, 2));

			Assert.AreEqual(350, multiplied.GetElement(1, 0));
			Assert.AreEqual(712, multiplied.GetElement(1, 1));
			Assert.AreEqual(1127, multiplied.GetElement(1, 2));

			Assert.AreEqual(109, multiplied.GetElement(2, 0));
			Assert.AreEqual(213, multiplied.GetElement(2, 1));
			Assert.AreEqual(728, multiplied.GetElement(2, 2));
		}

		#endregion
	}
}
