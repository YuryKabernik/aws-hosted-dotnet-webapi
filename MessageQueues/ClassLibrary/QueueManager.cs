using System;
using System.Messaging;
using System.Diagnostics;
using ClassLibrary.Models;
using System.Linq;

namespace ClassLibrary
{
	/// <summary>
	/// Queue manipulation and messaging.
	/// </summary>
	public static class QueueManager
	{
		private const int MAX_CHUNK_SIZE = 1000000;

		/// <summary>
		/// Message queue object creation.
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public static MessageQueue CreateMQ(string queueName)
		{
			MessageQueue queue;

			if (MessageQueue.Exists(queueName))
			{
				queue = new MessageQueue(queueName);
			}
			else
			{
				queue = MessageQueue.Create(queueName);
			}

			queue.Formatter = new XmlMessageFormatter(new Type[] {
				typeof(byte[]),
				typeof(string),
				typeof(int),
				typeof(FileMessage),
				typeof(QueueMessageChunk)
			});

			return queue;
		}

		/// <summary>
		/// Writing message to the queue.
		/// </summary>
		/// <param name="queue"></param>
		/// <param name="message"></param>
		public static void WriteMessage(MessageQueue queue, FileMessage message)
		{
			if (queue.CanWrite)
			{
				Guid sequenceGuid = Guid.NewGuid();
				int countOfChunks = (message.Content.Length / MAX_CHUNK_SIZE) + 1;

				if (countOfChunks == 1)
				{
					queue.Send(message);

					return;
				}

				var chunks = Enumerable.Range(1, countOfChunks)
					.Select(index => new QueueMessageChunk
					{
						Sequence = sequenceGuid,
						Position = (index - 1) * MAX_CHUNK_SIZE,
						Size = message.Content.Length,
						Message = new FileMessage
						{
							User = message.User,
							FileName = message.FileName,
							Content = message.Content
								.Skip((index - 1) * MAX_CHUNK_SIZE)
								.Take(MAX_CHUNK_SIZE)
								.ToArray()
						}
					})
					.ToList();

				foreach (var chunk in chunks)
				{
					queue.Send(chunk);
				}
			}
		}

		/// <summary>
		/// Reading last message from the queue.
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		public static object ReadMessage(MessageQueue queue)
		{
			if (queue.CanRead)
			{
				var res = queue.Receive();

				if (res.Body is FileMessage message)
				{
					Console.WriteLine(res.Id + " File name: " + message.FileName);

					return message;
				}
				if (res.Body is QueueMessageChunk messageChunk)
				{
					Console.WriteLine(res.Id + " Sequence number: " + messageChunk.Sequence + " File name: " + messageChunk.Message.FileName);

					return messageChunk;
				}
				else
				{
					Console.WriteLine("ERROR: response body is invalid ---> Response ID: {0}", res.Id);
				}
			}

			return new object();
		}
	}
}
