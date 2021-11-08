using System.Messaging;
using System.Threading;
using ClassLibrary;
using ClassLibrary.Models;

namespace CentralServer_Framework
{
	internal class Startup
	{
		internal static void Run(string currentDirectory)
		{
			string MessageQueueName = @".\private$\MyPrivateQueue";

			DirectoryManager directoryManager = new DirectoryManager(currentDirectory);
			MessageQueue queue = QueueManager.CreateMQ(MessageQueueName);

			ThreadPool.QueueUserWorkItem(obj =>
			{
				while (queue.CanRead)
				{
					var message = QueueManager.ReadMessage(queue);

					HandleMessage(directoryManager, message);
				}
			});
		}

		internal static void HandleMessage(DirectoryManager directoryManager, object message)
		{
			if (message is FileMessage fileMessage)
			{
				directoryManager.SaveFile(fileMessage.FileName, fileMessage.Content, fileMessage.User);
			}
			else if (message is QueueMessageChunk messageChunk)
			{
				directoryManager.SaveFileChunk(
					messageChunk.Message.FileName,
					messageChunk.Position,
					messageChunk.Message.Content,
					messageChunk.Message.User
				);
			}
		}
	}
}