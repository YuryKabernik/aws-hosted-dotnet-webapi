/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
	class Program
	{
		static CancellationTokenSource cancellationTokenSource;

		static void Main(string[] args)
		{
			Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
			Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
			Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
			Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
			Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
			Console.WriteLine("Demonstrate the work of the each case with console utility.");
			Console.WriteLine();

			Task.Run(TaskA)
				.ContinueWith(TaskB)
				.ContinueWith(TaskC)
				.ContinueWith(TaskD);

			Console.ReadLine();
		}


		static void TaskA()
		{
			Console.WriteLine("a.Continuation task should be executed regardless of the result of the parent task.");
			cancellationTokenSource = new CancellationTokenSource();

			Task.Run(() => SuccessAction(cancellationTokenSource.Token), cancellationTokenSource.Token)
				.ContinueWith(ContinuationAction)
				.Wait();

			Task.Run(() => FailedAction())
				.ContinueWith(ContinuationAction)
				.Wait();
		}

		static void TaskB(Task continuation)
		{
			Console.WriteLine("b.Continuation task should be executed when the parent task finished without success.");
			cancellationTokenSource = new CancellationTokenSource();

			try
			{
				Task.Run(() => SuccessAction(cancellationTokenSource.Token), cancellationTokenSource.Token)
					.ContinueWith(ContinuationAction, TaskContinuationOptions.NotOnRanToCompletion)
					.Wait();
			}
			catch (AggregateException) { }

			Task.Run(() => FailedAction())
				.ContinueWith(ContinuationAction, TaskContinuationOptions.NotOnRanToCompletion)
				.Wait();
		}

		static void TaskC(Task continuation)
		{
			Console.WriteLine("c.Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
			cancellationTokenSource = new CancellationTokenSource();

			try
			{
				Task.Run(() => SuccessAction(cancellationTokenSource.Token), cancellationTokenSource.Token)
					.ContinueWith(ContinuationAction, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously)
					.Wait();
			}
			catch (AggregateException) { }

			Task.Run(() => FailedAction())
				.ContinueWith(ContinuationAction, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously)
				.Wait();
		}

		static void TaskD(Task continuation)
		{
			Console.WriteLine("d.Continuation task should be executed outside of the thread pool when the parent task would be cancelled");
			cancellationTokenSource = new CancellationTokenSource();

			try
			{
				Task.Run(() => SuccessAction(cancellationTokenSource.Token))
					.ContinueWith(ContinuationAction, TaskContinuationOptions.OnlyOnCanceled)
					.Wait();
			}
			catch (AggregateException) { }

			try
			{
				Task task = Task.Run(() => SuccessAction(cancellationTokenSource.Token), cancellationTokenSource.Token)
					.ContinueWith(ContinuationAction, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);
				cancellationTokenSource.Cancel();
				task.Wait();
			}
			catch (AggregateException) { }
		}

		static void SuccessAction(CancellationToken cancellationToken)
		{
			Console.WriteLine($"SuccessAction started in: {Thread.CurrentThread.ManagedThreadId} thread");
			if (cancellationToken.IsCancellationRequested)
			{
				Console.WriteLine("SuccessAction cancelled");
				return;
			}

			Console.WriteLine("SuccessAction finished");
			Console.WriteLine();
		}

		static void FailedAction()
		{
			Console.WriteLine($"FailedAction started in: {Thread.CurrentThread.ManagedThreadId} thread");
			Console.WriteLine();
			throw new Exception();
		}

		static void ContinuationAction(Task prevTask)
		{
			Console.WriteLine($"ContinuationAction started in: {Thread.CurrentThread.ManagedThreadId} thread");
			Console.WriteLine(prevTask.Status.ToString());
			Console.WriteLine();
		}
	}
}
