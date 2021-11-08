using System;

namespace ClassLibrary.Models
{
	public class QueueMessageChunk
	{
		/// <summary>
		/// The ID value of the sequence.
		/// </summary>
		public Guid Sequence { get; set; }
		
		/// <summary>
		/// The position of data in the sequence.
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// The size of data content.
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// The body of the chunk.
		/// </summary>
		public FileMessage Message { get; set; }
	}
}
