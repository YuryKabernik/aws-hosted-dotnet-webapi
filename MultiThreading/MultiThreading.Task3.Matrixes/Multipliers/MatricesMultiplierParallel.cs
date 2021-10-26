using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MultiThreading.Task3.MatrixMultiplier.Matrices;

namespace MultiThreading.Task3.MatrixMultiplier.Multipliers
{
	public class MatricesMultiplierParallel : IMatricesMultiplier
	{
		private readonly object writeResultLock = new object();

		public IMatrix Multiply(IMatrix m1, IMatrix m2)
		{
			IMatrix resultMatrix = new Matrix(m1.RowCount, m2.ColCount);

			Parallel.For(0, resultMatrix.RowCount, rowIndex =>
			{
				Parallel.For(0, resultMatrix.ColCount, colIndex =>
				{
					IList<long> m1Row = this.GetRow(m1, rowIndex);
					IList<long> m2Col = this.GetCol(m2, colIndex);

					long value = this.CalculateNode(m1Row, m2Col);

					this.SetValue(resultMatrix, rowIndex, colIndex, value);
				});
			});

			return resultMatrix;
		}

		private IList<long> GetRow(IMatrix matrix, long index)
		{
			IList<long> row = new List<long>();

			for (int i = 0; i < matrix.ColCount; i++)
			{
				row.Add(matrix.GetElement(index, i));
			}

			return row;
		}

		private IList<long> GetCol(IMatrix matrix, long index)
		{
			IList<long> column = new List<long>();

			for (int i = 0; i < matrix.RowCount; i++)
			{
				column.Add(matrix.GetElement(i, index));
			}

			return column;
		}

		private long CalculateNode(IList<long> m1Row, IList<long> m2Col)
		{
			long sum = 0;
			for (byte k = 0; k < m1Row.Count(); k++)
			{
				sum += m1Row[k] * m2Col[k];
			}

			return sum;
		}

		private void SetValue(IMatrix matrix, long row, long col, long value)
		{
			lock (this.writeResultLock)
			{
				matrix.SetElement(row, col, value);
			}
		}
	}
}
