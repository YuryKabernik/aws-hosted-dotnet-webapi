using System;
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
			string passwordHash = GeneratePasswordHashUsingSalt(password, saltBytes);

			Console.WriteLine("Password hash: {0}", passwordHash);
			Console.ReadKey();
		}

		public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
		{
			var iterate = 10000;
			var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);

			byte[] hash = pbkdf2.GetBytes(20);
			byte[] hashBytes = new byte[36];

			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			var passwordHash = Convert.ToBase64String(hashBytes);

			return passwordHash;
		}
	}
}
