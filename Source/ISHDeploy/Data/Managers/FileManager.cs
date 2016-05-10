using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Wrapper around .Net file system operations
    /// </summary>
    public class FileManager : IFileManager
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public FileManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Copies source file to destination file
        /// </summary>
        /// <param name="sourceFilePath">The file to copy.</param>
        /// <param name="destFilePath">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite">True if the destination file can be overwritten; otherwise False. </param>
        public void Copy(string sourceFilePath, string destFilePath, bool overwrite = false)
        {
			_logger.WriteDebug($"Copying file `{sourceFilePath}` to `{destFilePath}`, with `overwrite` option set to `{overwrite}`");
			File.Copy(sourceFilePath, destFilePath, overwrite);
        }

        /// <summary>
        /// Copies source file to destination directory
        /// </summary>
        /// <param name="sourceFilePath">The file to copy.</param>
        /// <param name="destDir">The name of the destination directory. This cannot be a file name.</param>
        /// <param name="overwrite">True if the destination file can be overwritten; otherwise False. </param>
        public void CopyToDirectory(string sourceFilePath, string destDir, bool overwrite = false)
        {
            Copy(sourceFilePath, Path.Combine(destDir, Path.GetFileName(sourceFilePath)), overwrite);
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>True if the caller has the required permissions and <paramref name="path"/> contains the name of an existing file</returns>
        public bool FileExists(string path)
        {
			return File.Exists(path);
        }

        /// <summary>
        /// Determines whether the specified folder exists.
        /// </summary>
        /// <param name="path">The folder to check.</param>
        /// <returns>True if folder exists</returns>
        public bool FolderExists(string path)
        {
			return Directory.Exists(path);
        }

        /// <summary>
        ///	Deletes file
        /// </summary>
        /// <param name="path">Path to the file to be deleted</param>
        public void Delete(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.IsReadOnly)
			{
				fileInfo.Attributes = FileAttributes.Normal;
			}

			_logger.WriteDebug($"Deliting file `{path}`");
			File.Delete(path);
        }

        /// <summary>
        /// Creates the folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be created</param>
        public void CreateDirectory(string folderPath)
        {
			_logger.WriteDebug($"Creating directory `{folderPath}`");
			Directory.CreateDirectory(folderPath);
        }

        /// <summary>
        /// Cleans up the folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be cleaned up</param>
        public void CleanFolder(string folderPath)
		{
			_logger.WriteDebug($"Cleaning up folder `{folderPath}`");
			if (this.FolderExists(folderPath))
			{
				foreach (string subFolderPath in Directory.GetDirectories(folderPath))
				{
					this.DeleteFolder(subFolderPath);
				}

				foreach (string filePath in Directory.GetFiles(folderPath))
				{
					this.Delete(filePath);
				}
			}
		}

		/// <summary>
		/// Deletes the folder
		/// </summary>
		/// <param name="folderPath">Path to folder to be deleted</param>
		/// <param name="recursive">True to remove directories, subdirectories, and files in path; otherwise False.</param>
		public void DeleteFolder(string folderPath, bool recursive = true)
		{
			_logger.WriteDebug($"Deleting folder `{folderPath}`{(recursive ? " recursively" : "")}");
			if (this.FolderExists(folderPath))
			{
				Directory.Delete(folderPath, recursive);
			}
		}

		/// <summary>
		/// Makes sure directory exists, if not, then creates it
		/// </summary>
		/// <param name="folderPath">Directory path to verify</param>
		public void EnsureDirectoryExists(string folderPath)
		{
			if (!this.FolderExists(folderPath))
			{
				_logger.WriteDebug($"Directory `{folderPath}` does not exist, creating it.");
				this.CreateDirectory(folderPath);
			}
			else
			{
				_logger.WriteDebug($"Directory `{folderPath}` exists");
			}
		}

		/// <summary>
		/// Copies content from one folder to another
		/// </summary>
		/// <param name="sourcePath">Source folder path</param>
		/// <param name="destinationPath">Destination folder path</param>
		public void CopyDirectoryContent(string sourcePath, string destinationPath)
		{
			_logger.WriteDebug($"Copying `{sourcePath}` directory content to {destinationPath}");
			if (this.FolderExists(sourcePath))
			{
				//Copy all the files & Replaces any files with the same name
				foreach (string newPath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
				{
					this.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
				}
			}
		}

		/// <summary>
		/// Opens a text file, reads all lines of the file, and then closes the file.
		/// </summary>
		/// <param name="filePath">The file to open for reading.</param>
		/// <returns>A string array containing all lines of the file.</returns>
		public string ReadAllText(string filePath)
        {
			_logger.WriteDebug($"Reading all text from file `{filePath}`");
			return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to open for reading.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        public string[] ReadAllLines(string filePath)
        {
			_logger.WriteDebug($"Reading all lines from file `{filePath}`");
			return File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Creates a new file, write the specified string array to the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to write to.</param>
        /// <param name="lines">The string array to write to the file.</param>
        public void WriteAllLines(string filePath, string[] lines)
        {
			_logger.WriteDebug($"Writing all lines to file `{filePath}`");
			File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Appends text to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="text">The text to be appended to the file content.</param>
        public void Append(string filePath, string text)
        {
			this.Write(filePath, text, true);
        }

		/// <summary>
		/// Writes text to the file. Creates new file if it does not exist.
		/// </summary>
		/// <param name="filePath">The file to open for writing.</param>
		/// <param name="text">Text to be appended to the file content.</param>
		/// <param name="append">True to append data to the file; false to overwrite the file.</param>
		public void Write(string filePath, string text, bool append = false)
		{
			_logger.WriteDebug($"{(append? "Appending" : "Writing")} content to file `{filePath}`");
			using (var fileStream = new StreamWriter(filePath, append))
			{
				fileStream.Write(text);
			}
		}

		/// <summary>
        /// Creates a new <see cref="XDocument" /> instance by using the specified file path.
		/// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new <see cref="XDocument" />.</param>
        /// <returns>New instance of <see cref="XDocument" /> with loaded file content</returns>
		public XDocument Load(string filePath)
        {
			_logger.WriteDebug($"Loading XML document at `{filePath}`");
			return XDocument.Load(filePath);
        }

        /// <summary>
        /// Creates a new <see cref="XmlDocument" /> instance by using the specified file path.
        /// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new <see cref="XmlDocument" />.</param>
        /// <returns>New instance of <see cref="XmlDocument" /> with loaded file content</returns>
        public XmlDocument LoadXmlDoc(string filePath)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);

            return doc;
        }

        /// <summary>
        /// Saves <see cref="XDocument" /> content to file
        /// </summary>
        /// <param name="filePath">The file where <see cref="XDocument" /> content will be stored.</param>
        /// <param name="doc">The document to be stored</param>
        public void Save(string filePath, XDocument doc)
        {
			_logger.WriteDebug($"Saving XML document to `{filePath}`");
			doc.Save(filePath);
        }

        /// <summary>
        /// Tries to find license file on the system
        /// </summary>
        /// <param name="licenseFolderPath">License file.</param>
        /// <param name="hostName">Host name.</param>
        /// <param name="licenseFileExtension">License file extension.</param>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns True if license file is found, otherwise False.</returns>
        public bool TryToFindLicenseFile(string licenseFolderPath, string hostName, string licenseFileExtension, out string filePath)
        {
			_logger.WriteDebug($"Looking for `{hostName}` license file");
			filePath = Path.Combine(licenseFolderPath, string.Concat(hostName, licenseFileExtension));

            if (File.Exists(filePath))
            {
				_logger.WriteDebug($"License file `{filePath}` found for `{hostName}`");
				return true;
            }

            int i = hostName.IndexOf(".", StringComparison.InvariantCulture);
            if (i > 0)
            {
                return TryToFindLicenseFile(licenseFolderPath, hostName.Substring(i + 1), licenseFileExtension, out filePath);
            }

			_logger.WriteDebug($"License file for `{hostName}` is not found.");
			return false;
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory, uses the Optimal compression level, and optionally includes the base directory.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path to the directory to be archived, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="destinationArchiveFilePath">The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="includeBaseDirectory">true to include the directory name from sourceDirectoryName at the root of the archive; false to include only the contents of the directory. True by default</param>
        public void PackageDirectory(string sourceDirectoryPath, string destinationArchiveFilePath, bool includeBaseDirectory = true)
        {
            _logger.WriteDebug($"Directory '{sourceDirectoryPath}' will be packed");

            if (FileExists(destinationArchiveFilePath))
            {
                _logger.WriteWarning($"Package file '{destinationArchiveFilePath}' is overwritten.");
                Delete(destinationArchiveFilePath);
            }

            ZipFile.CreateFromDirectory(sourceDirectoryPath, destinationArchiveFilePath, CompressionLevel.Optimal, includeBaseDirectory);

            _logger.WriteDebug($"The output package is: '{destinationArchiveFilePath}'");
        }
    }
}
