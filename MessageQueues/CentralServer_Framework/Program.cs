using System;

namespace CentralServer_Framework
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Running server...");

			Startup.Run(Environment.CurrentDirectory);

			Console.WriteLine("Server has been started.");
			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();
		}
	}
}
