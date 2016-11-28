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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            _logger.WriteDebug($"Copy file `{sourceFilePath}` to `{destFilePath}`, with `overwrite` option set to `{overwrite}`");
            File.Copy(sourceFilePath, destFilePath, overwrite);
            _logger.WriteVerbose($"The file `{sourceFilePath}` has been copied to `{destFilePath}`");
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

            _logger.WriteDebug("Delete file", path);
            File.Delete(path);
            _logger.WriteVerbose($"The file `{path}` has been deleted");
        }

        /// <summary>
        /// Creates the folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be created</param>
        public void CreateDirectory(string folderPath)
        {
            _logger.WriteDebug("Create directory", folderPath);
            Directory.CreateDirectory(folderPath);
            _logger.WriteVerbose($"The folder `{folderPath}` has been created");
        }

        /// <summary>
        /// Cleans up the folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be cleaned up</param>
        public void CleanFolder(string folderPath)
        {
            _logger.WriteDebug("Clean folder", folderPath);
            if (FolderExists(folderPath))
            {
                foreach (string subFolderPath in Directory.GetDirectories(folderPath))
                {
                    DeleteFolder(subFolderPath);
                }

                foreach (string filePath in GetFiles(folderPath, "*", false))
                {
                    Delete(filePath);
                }
            }
            _logger.WriteVerbose($"The folder `{folderPath}` has been cleaned");
        }

        /// <summary>
        /// Deletes the folder
        /// </summary>
        /// <param name="folderPath">Path to folder to be deleted</param>
        public void DeleteFolder(string folderPath)
        {
            _logger.WriteDebug("Delete folder", folderPath);
            if (FolderExists(folderPath))
            {
                // known c# issue
                // TS-11684
                // http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
                DeleteDirectory(folderPath);
            }
            _logger.WriteVerbose($"Deleted folder `{folderPath}`");
        }

        /// <summary>
        /// Depth-first recursive delete, with handling for descendant 
        /// directories open in Windows Explorer.
        /// </summary>
        private void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Makes sure directory exists, if not, then creates it
        /// </summary>
        /// <param name="folderPath">Directory path to verify</param>
        public void EnsureDirectoryExists(string folderPath)
        {
            if (!FolderExists(folderPath))
            {
                _logger.WriteDebug("The folder not exists, creating it", folderPath);
                CreateDirectory(folderPath);
            }
            else
            {
                _logger.WriteDebug("The folder exists", folderPath);
            }
        }

        /// <summary>
        /// Copies content from one folder to another
        /// </summary>
        /// <param name="sourcePath">Source folder path</param>
        /// <param name="destinationPath">Destination folder path</param>
        public void CopyDirectoryContent(string sourcePath, string destinationPath)
        {
            _logger.WriteDebug($"Copy directory content from `{sourcePath}` to `{destinationPath}`");
            if (FolderExists(sourcePath))
            {
                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in GetFiles(sourcePath, "*", true))
                {
                    Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
            }
            _logger.WriteVerbose($"The content of the folder `{sourcePath}` has been copied to {destinationPath}");
        }

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to open for reading.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        public string ReadAllText(string filePath)
        {
            _logger.WriteDebug("Read all text", filePath);
            var text = File.ReadAllText(filePath);
            _logger.WriteVerbose($"All text from file `{filePath}` has been read");
            return text;
        }

        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to open for reading.</param>
        /// <returns>A string array containing all lines of the file.</returns>
        public string[] ReadAllLines(string filePath)
        {
            _logger.WriteDebug("Read all lines", filePath);
            var lines = File.ReadAllLines(filePath);
            _logger.WriteVerbose($"All lines from file `{filePath}` have been read");
            return lines;
        }

        /// <summary>
        /// Creates a new file, write the specified string array to the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The file to write to.</param>
        /// <param name="lines">The string array to write to the file.</param>
        public void WriteAllLines(string filePath, string[] lines)
        {
            _logger.WriteDebug("Write all lines", filePath);
            File.WriteAllLines(filePath, lines);
            _logger.WriteVerbose($"All lines have been written to file `{filePath}`");
        }

        /// <summary>
        /// Appends text to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="text">The text to be appended to the file content.</param>
        public void Append(string filePath, string text)
        {
            Write(filePath, text, true);
        }

        /// <summary>
        /// Writes text to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="text">Text to be appended to the file content.</param>
        /// <param name="append">True to append data to the file; false to overwrite the file.</param>
        public void Write(string filePath, string text, bool append = false)
        {
            _logger.WriteDebug($"{(append ? "Append" : "Write")} content", filePath);

            EnsureDirectoryExists(Path.GetDirectoryName(filePath));

            using (var fileStream = new StreamWriter(filePath, append))
            {
                fileStream.Write(text);
            }
            _logger.WriteVerbose($"The content has been {(append ? "appended" : "written")} to file `{filePath}`");
        }

        /// <summary>
        /// Version of History file
        /// </summary>
        private readonly string _historyFileVersion = "1.0";

        /// <summary>
        /// Header params
        /// </summary>
        private readonly string _headerParams =
@"param(
    [Parameter(Mandatory=$false)]
    [switch]$IncludeCustomFile=$false
)

";
        /// <summary>
        /// Create history header
        /// </summary>
        private string GetHeader(string name, string currentVersion, string updatedVersion)
        {
            return
 $@"<#ISHDeployScriptInfo

.VERSION {_historyFileVersion}

.MODULE {name}

.CREATEDBYMODULEVERSION {currentVersion}

.UPDATEDBYMODULEVERSION {updatedVersion}

#>

" + _headerParams;
        }

        /// <summary>
        /// Writes text header to the file. Creates new file if it does not exist.
        /// </summary>
        /// <param name="filePath">The file to open for writing.</param>
        /// <param name="version">Module version.</param>
        public void WriteHistoryHeader(string filePath, Version version)
        {
            _logger.WriteDebug($"Write header", filePath);

            EnsureDirectoryExists(Path.GetDirectoryName(filePath));

            string currentContent = String.Empty;
            if (File.Exists(filePath))
            {
                currentContent = File.ReadAllText(filePath);
            }

            string createdModuleVersion, currentModuleVersion;
            createdModuleVersion = currentModuleVersion = version.ToString();

            var groups = Regex.Match(currentContent, @".CREATEDBYMODULEVERSION\((.*?)\)").Groups;
            if (groups[1].Success != false)
            { //header already exist
                createdModuleVersion = groups[1].Value;
                int found = currentContent.IndexOf(_headerParams);
                if (found != -1)
                {
                    currentContent = currentContent.Substring(found + _headerParams.Length);
                }
            }

            var header = GetHeader(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                createdModuleVersion,
                currentModuleVersion);


            File.WriteAllText(filePath, header + currentContent);

            _logger.WriteVerbose($"The header has been added to file `{filePath}`");
        }

        /// <summary>
        /// Creates a new <see cref="XDocument" /> instance by using the specified file path.
        /// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new <see cref="XDocument" />.</param>
        /// <returns>New instance of <see cref="XDocument" /> with loaded file content</returns>
        public XDocument Load(string filePath)
        {
            _logger.WriteDebug("Load XML document", filePath);
            var doc = XDocument.Load(filePath);
            _logger.WriteVerbose($"The XML document from file `{filePath}` has been loaded");
            return doc;
        }

        /// <summary>
        /// Saves <see cref="XDocument" /> content to file
        /// </summary>
        /// <param name="filePath">The file where <see cref="XDocument" /> content will be stored.</param>
        /// <param name="doc">The document to be stored</param>
        public void Save(string filePath, XDocument doc)
        {
            _logger.WriteDebug("Save XML document", filePath);
            doc.Save(filePath);
            _logger.WriteVerbose($"The XML document has been saved to file `{filePath}`");
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
            _logger.WriteDebug("Look license file", hostName);
            filePath = Path.Combine(licenseFolderPath, string.Concat(hostName, licenseFileExtension));

            if (File.Exists(filePath))
            {
                _logger.WriteVerbose($"The license file `{filePath}` has been found for `{hostName}`");
                return true;
            }

            var isFound = false;
            int i = hostName.IndexOf(".", StringComparison.InvariantCulture);
            if (i > 0)
            {
                isFound = TryToFindLicenseFile(licenseFolderPath, hostName.Substring(i + 1), licenseFileExtension, out filePath);
            }

            return isFound;
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory, uses the Optimal compression level, and optionally includes the base directory.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path to the directory to be archived, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="destinationArchiveFilePath">The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="includeBaseDirectory">'True' to include the directory name from sourceDirectoryName at the root of the archive; 'False' to include only the contents of the directory. 'False' by default</param>
        public void PackageDirectory(string sourceDirectoryPath, string destinationArchiveFilePath, bool includeBaseDirectory = false)
        {
            _logger.WriteDebug("Pack folder", sourceDirectoryPath);

            if (FileExists(destinationArchiveFilePath))
            {
                _logger.WriteWarning($"Package file '{destinationArchiveFilePath}' is overwritten.");
                Delete(destinationArchiveFilePath);
            }

            var destinationArchiveFolderPath = Path.GetDirectoryName(destinationArchiveFilePath);
            EnsureDirectoryExists(destinationArchiveFolderPath);

            ZipFile.CreateFromDirectory(sourceDirectoryPath, destinationArchiveFilePath, CompressionLevel.Optimal, includeBaseDirectory);

            _logger.WriteVerbose($"The output package `{destinationArchiveFilePath}` has been created");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceArchiveFilePath">The path to archive.</param>
        /// <param name="destinationDirectoryPath">The path to destination folder.</param>
        public void ExtractPackageToDirectory(string sourceArchiveFilePath, string destinationDirectoryPath)
        {
            _logger.WriteDebug("Unzip folder", sourceArchiveFilePath, destinationDirectoryPath);

            if (FolderExists(destinationDirectoryPath))
            {
                DeleteDirectory(destinationDirectoryPath);
            }

            ZipFile.ExtractToDirectory(sourceArchiveFilePath, destinationDirectoryPath);

            _logger.WriteVerbose($"The source package `{sourceArchiveFilePath}` has been extracted to `{destinationDirectoryPath}`");
        }

        /// <summary>
        /// Determines whether is the specified file locked.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public bool IsFileLocked(string filePath)
        {
            _logger.WriteDebug("Check file locked or not", filePath);
            var fileInfo = new FileInfo(filePath);
            FileStream stream = null;

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File availability check failed. Could not find file `{filePath}`", filePath);
            }

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                _logger.WriteVerbose($"The `{filePath}` file is locked");
                return true;
            }
            finally
            {
                stream?.Close();
            }

            _logger.WriteVerbose($"The `{filePath}` file is available to access");
            return false;
        }

        /// <summary>
        /// Assigns the permissions for directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="user">The user.</param>
        /// <param name="fileSystemRights">The file system rights.</param>
        /// <param name="inheritanceFlags">The inheritance flags.</param>
        /// <param name="propagationFlags">The propagation flags.</param>
        /// <param name="type">The type.</param>
        public void AssignPermissionsForDirectory(string path, string user, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
        {
            _logger.WriteDebug("Assign permissions to folder", user, path);
            var accessRule = new FileSystemAccessRule(user, fileSystemRights,
                inheritanceFlags, propagationFlags,
                type);

            var directoryInfo = new DirectoryInfo(path);

            var security = directoryInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            security.AddAccessRule(accessRule);

            // Set the new access settings.
            directoryInfo.SetAccessControl(security);
            _logger.WriteVerbose($"Permissions to the folder `{path}` for user `{user}` have been assigned");
        }

        /// <summary>
        /// Assigns the permissions for file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="user">The user.</param>
        /// <param name="fileSystemRights">The file system rights.</param>
        /// <param name="type">The type.</param>
        public void AssignPermissionsForFile(string path, string user, FileSystemRights fileSystemRights, AccessControlType type)
        {
            _logger.WriteDebug("Assign permissions to file", user, path);
            var accessRule = new FileSystemAccessRule(user, fileSystemRights, type);

            var fileInfo = new FileInfo(path);

            var security = fileInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            security.AddAccessRule(accessRule);

            // Set the new access settings.
            fileInfo.SetAccessControl(security);
            _logger.WriteVerbose($"Permissions to the file `{path}` for user `{user}` have been assigned");
        }

        /// <summary>
        /// Gets list of files
        /// </summary>
        /// <param name="path">The path to directory.</param>
        /// <param name="searchPattern">The pattern to search.</param>
        /// <param name="recurse">Search in all directories or just in top one.</param>
        /// <returns></returns>
        public List<string> GetFiles(string path, string searchPattern, bool recurse)
        {
            List<string> list;
            if (recurse)
            {
                _logger.WriteDebug("Get list of files", searchPattern, path, SearchOption.AllDirectories);
                list = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories).ToList();
                _logger.WriteVerbose($"The list of all `{searchPattern}` files in folder `{path}` and in all sub-folders has been got");
            }
            else
            {
                _logger.WriteDebug("Get list of files", searchPattern, path);
                list = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly).ToList();
                _logger.WriteVerbose($"The list of all `{searchPattern}` files in folder `{path}` has been got");
            }
            return list;
        }

        /// <summary>
        /// Gets list of system entries
        /// </summary>
        /// <param name="path">The path to directory.</param>
        /// <param name="searchPattern">The pattern to search.</param>
        /// <param name="searchOption">Search option.</param>
        /// <returns></returns>
        public string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            _logger.WriteDebug("Get list of system entries", searchPattern, path);
            return Directory.GetFileSystemEntries(path, searchPattern, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Copy files with directory and file template.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path to source directory</param>
        /// <param name="destinationDirectoryPath">The path to source directory</param>
        /// <param name="searchPattern">The search pattern</param>
        public void CopyWithTemplate(string sourceDirectoryPath, string destinationDirectoryPath, string searchPattern)
        {
            Func<string[]> getFiles;

            if (string.IsNullOrEmpty(searchPattern))
            {
                getFiles = () => Directory.GetFiles(sourceDirectoryPath, "*.*");
            }
            else
            {
                string directoryTepmlate = Path.GetDirectoryName(searchPattern);
                string fileTemplate = Path.GetFileName(searchPattern);

                // For directories such as "Author\ASP\bin"
                if (Directory.Exists(Path.Combine(sourceDirectoryPath, searchPattern))) {
                    directoryTepmlate = Path.Combine(directoryTepmlate, fileTemplate);
                    fileTemplate = "";
                }
                
                if (string.IsNullOrEmpty(fileTemplate))
                    getFiles = () => Directory.GetFiles(
                        Path.Combine(sourceDirectoryPath, directoryTepmlate),
                        "*.*", SearchOption.AllDirectories);
                else
                    getFiles = () => Directory.GetFiles(
                        Path.Combine(sourceDirectoryPath, directoryTepmlate),
                        fileTemplate);
            }

            string[] files = getFiles();

            files
                .ToList()
                .ForEach(newPath =>
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath.Replace(sourceDirectoryPath, destinationDirectoryPath)));
                        File.Copy(newPath, newPath.Replace(sourceDirectoryPath, destinationDirectoryPath), true);// if already exist we do not copy - it is already backuped
                        _logger.WriteDebug($"File {newPath} was backuped");
                    });
        }
    }
}
