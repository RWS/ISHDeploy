using System.Xml.Linq;

namespace InfoShare.Deployment.Data.Managers.Interfaces
{
    /// <summary>
    /// Wrappper around .Net file system operations
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Copies source file to destination file
        /// </summary>
        /// <param name="sourceFilePath">The file to copy.</param>
        /// <param name="destFilePath">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false. </param>
        void Copy(string sourceFilePath, string destFilePath, bool overwrite = false);

        /// <summary>
        /// Copies source file to destination directory
        /// </summary>
        /// <param name="sourceFilePath">The file to copy.</param>
        /// <param name="destDir">The name of the destination directory. This cannot be a file name.</param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false. </param>
        void CopyToDirectory(string sourceFilePath, string destDir, bool overwrite = false);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the caller has the required permissions and <paramref name="path"/> contains the name of an existing file</returns>
        bool Exists(string path);

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to open for reading.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        string ReadAllText(string filePath);

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to open for reading.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        string[] ReadAllLines(string filePath);

        /// <summary>
        /// Creates a new file, write the specified string array to the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to write to.</param>
        /// <param name="lines">The string array to write to the file.</param>
        void WriteAllLines(string filePath, string[] lines);

        /// <summary>
        /// Appends text to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="text">the text to be appended to the file content.</param>
        void Append(string filePath, string text);

        /// <summary>
        /// Creates a new <see cref="T:System.Xml.Linq.XDocument"/> instance by using the specified stream.
        /// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new <see cref="T:System.Xml.Linq.XDocument"/>.</param>
        /// <returns>New instance of <see cref="T:System.Xml.Linq.XDocument"/> with loaded file content</returns>
        XDocument Load(string filePath);
        
        /// <summary>
        /// Saves <see cref="T:System.Xml.Linq.XDocument"/> content to file
        /// </summary>
        /// <param name="filePath">The file where <see cref="T:System.Xml.Linq.XDocument"/> content will be stored.</param>
        /// <param name="doc">The document to be stored</param>
        void Save(string filePath, XDocument doc);

		/// <summary>
		/// Deletes file from filesystem
		/// </summary>
		/// <param name="filePath">The file to delete.</param>
		void Delete(string filePath);

	    /// <summary>
	    /// Cleans up the folder
	    /// </summary>
	    /// <param name="folderPath">Path to folder to be cleaned up</param>
	    void CleanFolder(string folderPath);

		/// <summary>
		/// Delete the folder
		/// </summary>
		/// <param name="folderPath">Path to folder to be deleted</param>
		/// <param name="recursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
		void DeleteFolder(string folderPath, bool recursive = true);

	    /// <summary>
	    /// Makes sure directory exists, if not, then creates it
	    /// </summary>
	    /// <param name="folderPath">Directory path to verify</param>
	    void EnsureDirectoryExists(string folderPath);

		/// <summary>
		/// Copies content from one folder to another
		/// </summary>
		/// <param name="sourcePath">Source folder path</param>
		/// <param name="destinationPath">Destination folder path</param>
		void CopyDirectoryContent(string sourcePath, string destinationPath);

		/// <summary>
		/// Writes text to the file. Creates new file if it does not exist.
		/// </summary>
		/// <param name="filePath">The file to open for writing.</param>
		/// <param name="text">Text to be appended to the file content.</param>
		/// <param name="append">true to append data to the file; false to overwrite the file</param>
		void Write(string filePath, string text, bool append = false);

		/// <summary>
		/// Tries to find license file on the system
		/// </summary>
		/// <param name="licenseFolderPath">License file.</param>
		/// <param name="hostName">Host name.</param>
		/// <param name="licenseFileExtension">License file extension.</param>
		/// <param name="filePath">File path.</param>
		/// <returns>Returns true if license file is found, otherwise false.</returns>
		bool TryToFindLicenseFile(string licenseFolderPath, string hostName, string licenseFileExtension, out string filePath);
    }
}
