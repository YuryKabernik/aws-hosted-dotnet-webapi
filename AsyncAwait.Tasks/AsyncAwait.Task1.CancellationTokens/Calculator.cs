using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens
{
	static class Calculator
	{
		public static long Calculate(int n, CancellationToken cancellationToken)
		{
			long sum = 0;

			try
			{
				for (int i = 0; i < n; i++)
				{
					cancellationToken.ThrowIfCancellationRequested();

					sum += (i + 1); // i + 1 is to allow 2147483647 (Max(Int32))

					Thread.Sleep(10);
				}
			}
			catch (System.OperationCanceledException) { }

			return sum;
		}

		public static Task<long> CalculateAsync(int n, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(value => Calculate((int)value, cancellationToken), n, cancellationToken);
		}
	}
}
