/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Security.AccessControl;
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
        /// Returns a list of files that correspond to search pattern.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path to source directory</param>
        /// <param name="searchPattern">The search pattern</param>
        /// <returns>A string array containing list of required files.</returns>
        string[] GetFilesByCustomSearchPattern(string sourceDirectoryPath, string searchPattern);

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
        /// Writes text header to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="version">Module version.</param>
        void WriteHistoryHeader(string filePath, Version version);

        /// <summary>
        /// Creates a new file, write the specified string array to the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to write to.</param>
        /// <param name="lines">The string array to write to the file.</param>
        void WriteAllLines(string filePath, string[] lines);

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">The path to file.</param>
        /// <param name="content">The string to write to the file</param>
        void WriteAllText(string filePath, string content);

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
		void DeleteFolder(string folderPath);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirectoryPath">The path to the directory to be archived, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="destinationDirectoryPath">The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        void ExtractPackageToDirectory(string sourceDirectoryPath, string destinationDirectoryPath);

        /// <summary>
        /// Determines whether is the specified file locked.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        bool IsFileLocked(string filePath);

        /// <summary>
        /// Assigns the permissions for directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="user">The user.</param>
        /// <param name="fileSystemRights">The file system rights.</param>
        /// <param name="inheritanceFlags">The inheritance flags.</param>
        /// <param name="propagationFlags">The propagation flags.</param>
        /// <param name="type">The type.</param>
        void AssignPermissionsForDirectory(string path, string user, FileSystemRights fileSystemRights,
            InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type);

        /// <summary>
        /// Assigns the permissions for file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="user">The user.</param>
        /// <param name="fileSystemRights">The file system rights.</param>
        /// <param name="type">The type.</param>
        void AssignPermissionsForFile(string path, string user, FileSystemRights fileSystemRights,
            AccessControlType type);

        /// <summary>
        /// Gets list of files
        /// </summary>
        /// <param name="path">The path to directory.</param>
        /// <param name="searchPattern">The pattern to search.</param>
        /// <param name="recurse">Search in all directories or just in top one.</param>
        /// <returns>A string array containing list of required files.</returns>
        string[] GetFiles(string path, string searchPattern, bool recurse);

        /// <summary>
        /// Gets list of system entries
        /// </summary>
        /// <param name="path">The path to directory.</param>
        /// <param name="searchPattern">The pattern to search.</param>
        /// <param name="searchOption">Search option.</param>
        /// <returns>A string array containing list of required file entries.</returns>
        string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption);

        /// <summary>
        /// Save an object like a file
        /// </summary>
        /// <param name="filePath">The path to file</param>
        /// <param name="data">The object</param>
        void SaveObjectToFile<T>(string filePath, T data);

        /// <summary>
        /// Read an object from file
        /// </summary>
        /// <param name="filePath">The path to file</param>
        /// <returns>Deserialized object of type T</returns>
        T ReadObjectFromFile<T>(string filePath);
    }
}
