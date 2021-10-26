/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
	class Program
	{
		static int _maxValue = 50;
		static int _numerOfItems = 5;
		static Random _randomGenerator = new Random();

		static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

		static IList<object> _list = new List<object>();

		static void Main(string[] args)
		{
			Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
			Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
			Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
			Console.WriteLine();

			List<Action<object>> actionCollection = Enumerable.Range(1, 4)
				.Select(index => index % 2 == 0 ? new Action<object>(WriteToCollection) : new Action<object>(ReadCollection)).ToList();

			// ThreadPool
			actionCollection.Select(action => new WaitCallback(action)).ToList()
				.ForEach(waitCallback => ThreadPool.QueueUserWorkItem(waitCallback, _list));

			// Thread Class
			actionCollection.Select(action => new Thread(new ParameterizedThreadStart(action))).ToList()
				.ForEach(thread => thread.Start(_list));

			// Tasks
			actionCollection.ForEach(action => Task.Factory.StartNew(action, _list));

			ReadCollection(_list);

			Console.ReadLine();
		}

		static void WriteToCollection(object state)
		{
			IList<object> collection = state as IList<object>;

			try
			{
				_semaphoreSlim.Wait();

				for (int i = 0; i < _numerOfItems; i++)
				{
					collection.Add(_randomGenerator.Next(_maxValue));
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}

		static void ReadCollection(object state)
		{
			IList<object> collection = state as IList<object>;
			try
			{
				_semaphoreSlim.Wait();

				Console.WriteLine("~~~~~~~~~~~");
				
				if(!collection.Any())
				{
					Console.WriteLine("~~~EMPTY~~~");
				}

				foreach (int item in collection)
				{
					Console.WriteLine(item);
				}
				Console.WriteLine("~~~~~~~~~~~");
				Console.WriteLine();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}
	}
}
