/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
	class Program
	{
		static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0);

		static void Main(string[] args)
		{
			Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
			Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
			Console.WriteLine("Implement all of the following options:");
			Console.WriteLine();
			Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
			Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

			Console.WriteLine();

			Console.WriteLine("--- Thread ---");
			CreateThreadsWithJoin(10);
			Console.WriteLine();

			Console.WriteLine("--- ThreadPool ---");
			CreateThreadsWithThreadPool(10);
			Console.WriteLine();

			Console.ReadLine();
		}

		static void CreateThreadsWithJoin(object state)
		{
			int threadIndex = (int)state;

			if (threadIndex == 0)
				return;

			int decrementedThreadIndex = threadIndex - 1;

			Thread thread = new Thread(CreateThreadsWithJoin);

			thread.Start(decrementedThreadIndex);
			thread.Join();

			Console.WriteLine($"Thread index is: {threadIndex}");
		}

		static void CreateThreadsWithThreadPool(int threadIndex)
		{
			int decrementedThreadIndex = threadIndex - 1;
			if (threadIndex > 0)
			{

				ThreadPool.QueueUserWorkItem<int>(CreateThreadsWithThreadPool, decrementedThreadIndex, true);
			}
			else
			{
				_semaphoreSlim.Release();
			}

			try
			{
				_semaphoreSlim.Wait();
				Console.WriteLine($"Thread index is: {threadIndex}");
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}
	}
}
