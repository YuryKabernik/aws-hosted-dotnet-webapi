using System.IO;

namespace ClassLibrary
{
	public class DirectoryManager
	{
		/// <summary>
		/// This is the file system watcher.
		/// </summary>
		private FileSystemWatcher _watcher;

		/// <summary>
		/// This is the path to observed directory.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// This is the folder to listen
		/// </summary>
		public string Folder { get; set; } = "Storage";

		/// <summary>
		/// This is the target file extention to monitor.
		/// </summary>
		public string FileExtention { get; set; } = ".txt";

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public DirectoryManager(string path)
		{
			this.Path = path;
		}

		/// <summary>
		/// Subscribe event handler on the file creation event.
		/// </summary>
		public void SubscribeToListen(FileSystemEventHandler eventHandler, string folder = "")
		{
			string targetDirectoryPath = this.CombinePath(folder);

			Directory.CreateDirectory(targetDirectoryPath);
			this._watcher = new FileSystemWatcher(targetDirectoryPath);

			this._watcher.Created += eventHandler;
			this._watcher.IncludeSubdirectories = true;
			this._watcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// This method saves the file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="content"></param>
		/// <param name="folder"></param>
		public void SaveFile(string fileName, byte[] content, string folder = "")
		{
			this.SaveFileChunk(fileName, 0, content, folder);
		}

		/// <summary>
		/// This method saves a chunk of data into the file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="streamPosition"></param>
		/// <param name="content"></param>
		/// <param name="folder"></param>
		public void SaveFileChunk(string fileName, int streamPosition, byte[] content, string folder = "")
		{
			string targetDirectoryPath = this.CombinePath(folder);
			string targetFilePath = System.IO.Path.Combine(targetDirectoryPath, fileName);
			
			Directory.CreateDirectory(targetDirectoryPath);

			using (var targetFileStream = File.OpenWrite(targetFilePath))
			{
				targetFileStream.Position = streamPosition;
				targetFileStream.Write(content, 0, content.Length);
			}
		}

		/// <summary>
		/// Creates a file path.
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		private string CombinePath(string folder)
		{
			string folderName = string.IsNullOrEmpty(folder) ? this.Folder : folder;
			string targetPath = System.IO.Path.Combine(this.Path, folderName);

			return targetPath;
		}
	}
}
