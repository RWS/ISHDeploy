using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Managers
{
    /// <summary>
    /// Wrappper around .Net file system operations
    /// </summary>
    public class FileManager : IFileManager
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Contstuctor that accepts logger instance
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
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false. </param>
        public void Copy(string sourceFilePath, string destFilePath, bool overwrite = false)
        {
            File.Copy(sourceFilePath, destFilePath, overwrite);
        }

        /// <summary>
        /// Copies source file to destination directory
        /// </summary>
        /// <param name="sourceFilePath">The file to copy.</param>
        /// <param name="destDir">The name of the destination directory. This cannot be a file name.</param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false. </param>
        public void CopyToDirectory(string sourceFilePath, string destDir, bool overwrite = false)
        {
            Copy(sourceFilePath, Path.Combine(destDir, Path.GetFileName(sourceFilePath)), overwrite);
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the caller has the required permissions and <paramref name="path"/> contains the name of an existing file</returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
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

			File.Delete(path);
		}

		/// <summary>
		/// Cleans up the folder
		/// </summary>
		/// <param name="folderPath">Path to folder to be cleaned up</param>
		public void CleanFolder(string folderPath)
		{
			if (Directory.Exists(folderPath))
			{
				// Means there is no backup folder needed, and we just have to clean the related folder on deployment
				foreach (string filePath in Directory.GetFiles(folderPath))
				{
					this.Delete(filePath);
				}
			}
		}

		/// <summary>
		/// Copies content from one folder to another
		/// </summary>
		/// <param name="sourcePath">Source folder path</param>
		/// <param name="destinationPath">Destination folder path</param>
		public void CopyDirectoryContent(string sourcePath, string destinationPath)
		{
			if (Directory.Exists(sourcePath))
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
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to open for reading.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        public string[] ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Creates a new file, write the specified string array to the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to write to.</param>
        /// <param name="lines">The string array to write to the file.</param>
        public void WriteAllLines(string filePath, string[] lines)
        {
            File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Appends line to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="line">the line to be appended to the file content.</param>
        public void AppendLine(string filePath, string line)
        {
            using (var fileStream = new StreamWriter(filePath, true))
            {
                fileStream.WriteLine(line);
            }
        }

        /// <summary>
        /// Creates a new <see cref="T:System.Xml.Linq.XDocument"/> instance by using the specified stream.
        /// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new <see cref="T:System.Xml.Linq.XDocument"/>.</param>
        /// <returns>New instance of <see cref="T:System.Xml.Linq.XDocument"/> with loaded file content</returns>
        public XDocument Load(string filePath)
        {
            return XDocument.Load(filePath);
        }

        /// <summary>
        /// Saves <see cref="T:System.Xml.Linq.XDocument"/> content to file
        /// </summary>
        /// <param name="filePath">The file where <see cref="T:System.Xml.Linq.XDocument"/> content will be stored.</param>
        /// <param name="doc">The document to be stored</param>
        public void Save(string filePath, XDocument doc)
        {
            doc.Save(filePath);
        }

        /// <summary>
        /// Tries to find license file on the system
        /// </summary>
        /// <param name="licenseFolderPath">License file.</param>
        /// <param name="hostName">Host name.</param>
        /// <param name="licenseFileExtension">License file extension.</param>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns true if license file is found, otherwise false.</returns>
        public bool TryToFindLicenseFile(string licenseFolderPath, string hostName, string licenseFileExtension, out string filePath)
        {
            filePath = Path.Combine(licenseFolderPath, string.Concat(hostName, licenseFileExtension));

            if (File.Exists(filePath))
                return true;

            int i = hostName.IndexOf(".", StringComparison.InvariantCulture);
            if (i > 0)
            {
                return TryToFindLicenseFile(licenseFolderPath, hostName.Substring(i + 1), licenseFileExtension, out filePath);
            }
            
            return false;
        }
    }
}
