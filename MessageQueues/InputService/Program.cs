using System;

namespace InputService
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello <username>, what's your name?");
			string name = Console.ReadLine();

			Startup.Run(Environment.CurrentDirectory, name);

			Console.WriteLine($"Registration comleted. Welcome, {name}!");
			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();
		}
	}
}
