using System.Configuration;
using System.IO;
using System.Messaging;
using System.Threading;
using ClassLibrary;
using ClassLibrary.Models;

namespace InputService
{
	public class Startup
	{
		internal static void Run(string currentDirectory, string name)
		{
			string MessageQueueName = ConfigurationManager.AppSettings["FileDeliveryQueueAddress"];

			DirectoryManager directoryManager = new DirectoryManager(currentDirectory);
			MessageQueue queue = QueueManager.CreateMQ(MessageQueueName);

			directoryManager.Folder = name;

			directoryManager.SubscribeToListen(
				(object sender, FileSystemEventArgs e) => SendDocToServer(queue, e.FullPath, directoryManager.Folder)
			);
		}

		private static void SendDocToServer(MessageQueue queue, string fullPath, string user)
		{
			ThreadPool.QueueUserWorkItem(obj =>
			{
				if (File.Exists(fullPath) && TryOpen(fullPath, 3))
				{
					FileMessage message = new FileMessage
					{
						FileName = Path.GetFileName(fullPath),
						Content = File.ReadAllBytes(fullPath),
						User = user
					};

					QueueManager.WriteMessage(queue, message);
				}
			});
		}

		private static bool TryOpen(string fileName, int tryCount)
		{
			for (int i = 0; i < tryCount; i++)
			{
				try
				{
					var file = File.OpenRead(fileName);
					file.Close();

					return true;
				}
				catch (IOException)
				{
					Thread.Sleep(5000);
				}
			}

			return false;
		}
	}
}
