/*
 * Изучите код данного приложения для расчета суммы целых чисел от 0 до N, а затем
 * измените код приложения таким образом, чтобы выполнялись следующие требования:
 * 1. Расчет должен производиться асинхронно.
 * 2. N задается пользователем из консоли. Пользователь вправе внести новую границу в процессе вычислений,
 * что должно привести к перезапуску расчета.
 * 3. При перезапуске расчета приложение должно продолжить работу без каких-либо сбоев.
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens
{
	class Program
	{
		private static CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// The Main method should not be changed at all.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
			Console.WriteLine("Calculating the sum of integers from 0 to N.");
			Console.WriteLine("Use 'q' key to exit...");
			Console.WriteLine();

			Console.WriteLine("Enter N: ");

			string input = Console.ReadLine();
			while (input.Trim().ToUpper() != "Q")
			{
				if (int.TryParse(input, out int n))
				{
					CalculateSum(n);
				}
				else
				{
					Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
					Console.WriteLine("Enter N: ");
				}

				input = Console.ReadLine();
			}

			Console.WriteLine("Press any key to continue");
			Console.ReadLine();
		}

		private static void CalculateSum(int n)
		{
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource?.Cancel();
				_cancellationTokenSource?.Dispose();
			}

			if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource = new CancellationTokenSource();
			}

			Task<long> sumTask = Calculator.CalculateAsync(n, _cancellationTokenSource.Token);

			sumTask.ContinueWith(sum => Console.WriteLine($"Sum for {n} = {sum.Result}.\n"),
				TaskContinuationOptions.OnlyOnRanToCompletion);

			sumTask.ContinueWith(sum => Console.WriteLine($"Sum for {n} cancelled..."),
				TaskContinuationOptions.OnlyOnCanceled);

			Console.WriteLine($"The task for {n} started... Enter N to cancel the request.");

			ReadCancellation();
		}

		private static void ReadCancellation()
		{
			string input = Console.ReadLine();

			if (int.TryParse(input, out int n)) // calculate new value
			{
				CalculateSum(n);
			}
			else if (input.Trim().Equals("N", StringComparison.OrdinalIgnoreCase))
			{
				_cancellationTokenSource?.Cancel();
				Console.WriteLine("Enter N: ");
			}
		}
	}
}