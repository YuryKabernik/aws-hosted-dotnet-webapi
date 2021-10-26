/*
 * 1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.
 * Each Task should iterate from 1 to 1000 and print into the console the following string:
 * “Task #0 – {iteration number}”.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task1._100Tasks
{
	class Program
	{
		const int TaskAmount = 100;
		const int MaxIterationsCount = 1000;

		static void Main(string[] args)
		{
			Console.WriteLine(".Net Mentoring Program. Multi threading V1.");
			Console.WriteLine("1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.");
			Console.WriteLine("Each Task should iterate from 1 to 1000 and print into the console the following string:");
			Console.WriteLine("“Task #0 – {iteration number}”.");
			Console.WriteLine();

			CreateHundredTasks();

			Console.ReadLine();
		}

		static void CreateHundredTasks()
		{
			IEnumerable<Task> tasks = Enumerable.Range(1, TaskAmount).Select(taskId =>
			{
				TaskState state = new TaskState { TaskId = taskId, MaxIterationsCount = MaxIterationsCount };

				return Task.Factory.StartNew(IterateThroughOneThousand, state);
			});

			Task.WaitAll(tasks.ToArray());
		}

		private static void IterateThroughOneThousand(object taskState)
		{
			TaskState state = taskState as TaskState;

			for (int iteration = 0; iteration < state.MaxIterationsCount; iteration++)
			{
				Output(state.TaskId, iteration);
			}
		}

		static void Output(int taskNumber, int iterationNumber)
		{
			Console.WriteLine($"Task #{taskNumber} – {iterationNumber}");
		}
	}
}
