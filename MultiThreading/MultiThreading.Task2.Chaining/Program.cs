/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
	class Program
	{
		private static int _minRandValue = 1;
		private static int _maxRandValue = 10;

		private static Random _randomGenerator { get; set; } = new Random();

		static void Main(string[] args)
		{
			Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
			Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
			Console.WriteLine("First Task – creates an array of 10 random integer.");
			Console.WriteLine("Second Task – multiplies this array with another random integer.");
			Console.WriteLine("Third Task – sorts this array by ascending.");
			Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
			Console.WriteLine();

			// feel free to add your code
			StartTaskChain();

			Console.ReadLine();
		}

		private static void StartTaskChain()
		{
			Task task = Task.Factory.StartNew(FirstTask)
				.ContinueWith(continuation => SecondTask(continuation.Result))
				.ContinueWith(continuation => ThirdTask(continuation.Result))
				.ContinueWith(continuation => FourthTask(continuation.Result))
				.ContinueWith(continuation => Console.WriteLine("Average value: {0}", continuation.Result));

			task.Wait();
		}

		/// <summary>
		/// Creates an array of 10 random integer.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<int> FirstTask()
		{
			IEnumerable<int> randomValues = Enumerable.Range(1, 10)
				.Select((index) => _randomGenerator.Next(_minRandValue, _maxRandValue))
				.ToList();

			PrintOutput("Random values:", randomValues);

			return randomValues;
		}

		/// <summary>
		/// Multiplies the array with another random integer.
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		private static IEnumerable<int> SecondTask(IEnumerable<int> array)
		{
			int randomInteger = _randomGenerator.Next(_minRandValue, _maxRandValue);
			IEnumerable<int> multipliedArray = array.Select(value => value * randomInteger).ToList();

			PrintOutput($"Multiplied values with {randomInteger} integer:", multipliedArray);

			return multipliedArray;
		}

		/// <summary>
		/// Sorts the array by ascending.
		/// </summary>
		/// <param name="array"></param>
		private static IEnumerable<int> ThirdTask(IEnumerable<int> array)
		{
			IEnumerable<int> sortedArray = array.OrderBy(integer => integer).ToList();

			PrintOutput("Sorted by ascending:", sortedArray);

			return sortedArray;
		}

		/// <summary>
		/// Calculates the average value.
		/// </summary>
		private static double FourthTask(IEnumerable<int> array)
		{
			double averageValue = array.Average();

			return averageValue;
		}

		/// <summary>
		///  All this tasks should print the values to console.
		/// </summary>
		private static void PrintOutput(string message, IEnumerable<int> array)
		{
			string resultValue = string.Join(", ", array);

			Console.WriteLine("{0} {1}", message, resultValue);
		}
	}
}
