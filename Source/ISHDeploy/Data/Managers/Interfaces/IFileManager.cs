using System.Xml.Linq;

namespace ISHDeploy.Data.Managers.Interfaces
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
        /// <param name="overwrite">True if the destination file can be overwritten; otherwise, False. </param>
        void Copy(string sourceFilePath, string destFilePath, bool overwrite = false);

        /// <summary>
        /// Copies source file to destination directory
        /// </summary>
        /// <param name="sourceFilePath">The file to copy.</param>
        /// <param name="destDir">The name of the destination directory. This cannot be a file name.</param>
        /// <param name="overwrite">True if the destination file can be overwritten; otherwise False. </param>
        void CopyToDirectory(string sourceFilePath, string destDir, bool overwrite = false);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the caller has the required permissions and <paramref name="path"/> contains the name of an existing file</returns>
        bool FileExists(string path);

        /// <summary>
        /// Determines whether the specified folder exists.
        /// </summary>
        /// <param name="path">The folder to check.</param>
        /// <returns>True if folder exists</returns>
        bool FolderExists(string path);

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
        /// Creates a new <see cref="XDocument"/> instance by using the specified file path.
        /// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new <see cref="XDocument"/>.</param>
        /// <returns>New instance of <see cref="XDocument"/> with loaded file content</returns>
        XDocument Load(string filePath);

        /// <summary>
        /// Saves <see cref="XDocument"/> content to file
        /// </summary>
        /// <param name="filePath">The file where <see cref="XDocument"/> content will be stored.</param>
        /// <param name="doc">The document to be stored</param>
        void Save(string filePath, XDocument doc);

		/// <summary>
		/// Deletes file from filesystem
		/// </summary>
		/// <param name="filePath">The file to delete.</param>
		void Delete(string filePath);

        /// <summary>
        /// Creates folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be created</param>
        void CreateDirectory(string folderPath);

        /// <summary>
        /// Cleans up the folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be cleaned up</param>
        void CleanFolder(string folderPath);

		/// <summary>
		/// Delete the folder
		/// </summary>
		/// <param name="folderPath">Path to folder to be deleted</param>
		/// <param name="recursive">True to remove directories, subdirectories, and files in path; otherwise, False.</param>
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
		/// <returns>Returns True if license file is found, otherwise False.</returns>
		bool TryToFindLicenseFile(string licenseFolderPath, string hostName, string licenseFileExtension, out string filePath);

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory, uses the Optimal compression level, and optionally includes the base directory.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path to the directory to be archived, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="destinationArchiveFilePath">The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="includeBaseDirectory">'True' to include the directory name from sourceDirectoryName at the root of the archive; 'False' to include only the contents of the directory. 'False' by default</param>
        void PackageDirectory(string sourceDirectoryPath, string destinationArchiveFilePath, bool includeBaseDirectory = false);
    }
}
