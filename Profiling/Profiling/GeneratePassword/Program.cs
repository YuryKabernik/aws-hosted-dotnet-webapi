using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GeneratePassword
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Enter password:");
			string password = Console.ReadLine();

			Console.WriteLine("Enter salt:");
			string salt = Console.ReadLine();
			byte[] saltBytes = Encoding.Unicode.GetBytes(salt);

			while (saltBytes.Length < 16)
			{
				Console.WriteLine("Incorrect salt! Enter 16 characters or more.");
				Console.WriteLine("Enter new salt:");

				salt = Console.ReadLine();
				saltBytes = Encoding.Unicode.GetBytes(salt);
			}

			Console.WriteLine("Processing started...");
			try
			{
				string passwordHash = GeneratePasswordHashUsingSalt(password, saltBytes);
				Console.WriteLine("Password hash: {0}", passwordHash);
			}
			catch (Exception)
			{
				Console.WriteLine("Application has run into a problem!!!");
			}

			Console.ReadKey();
		}

		public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
		{
			var iterate = 10000;
			var passwordHash = string.Empty;
			var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);

			using (pbkdf2) // dispose IDisposable object
			{
				IEnumerable<byte> hash = pbkdf2.GetBytes(20); // ~49ms here to get bytes + 6 MB memory consumed (view Diagnostics Tool)
				IEnumerable<byte> hashBytes = Enumerable.Concat(salt, hash); // concat instead of copy

				passwordHash = Convert.ToBase64String(hashBytes.ToArray());
			}

			return passwordHash;
		}
	}
}
