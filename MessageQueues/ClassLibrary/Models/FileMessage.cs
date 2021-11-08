using System.IO;

namespace ClassLibrary.Models
{
	public class FileMessage
	{
		public string FileName { get; set; }

		public byte[] Content { get; set; }

		public string User { get; set; }
	}
}
