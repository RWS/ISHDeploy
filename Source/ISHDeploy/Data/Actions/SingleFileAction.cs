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
﻿using System;
using System.IO;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions
{
	/// <summary>
	/// Class to do single file manipulations
	/// </summary>
    public abstract class SingleFileAction : BaseAction, IRestorableAction, IDisposable
	{
		/// <summary>
		/// Extension for backup files
		/// </summary>
		private const string BackUpFileExtension = ".bak";

		/// <summary>
		/// IshFilePath instance, containing relative path, deployment and deployment type
		/// </summary>
		protected readonly ISHFilePath IshFilePath;

		/// <summary>
		/// Path to back up file
		/// </summary>
		protected string BackupPath;

		/// <summary>
		/// File Manager instance
		/// </summary>
		protected readonly IFileManager FileManager;

		/// <summary>
		/// Absolute path to file
		/// </summary>
	    protected string FilePath => IshFilePath.AbsolutePath;

		/// <summary>
		/// Implements single file action constructor
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="ishFilePath">Wrapper for file path</param>
		protected SingleFileAction(ILogger logger, ISHFilePath ishFilePath)
			: base(logger)
        {
			IshFilePath = ishFilePath;
            FileManager = ObjectFactory.GetInstance<IFileManager>();

            // Make sure Vanilla backup exists
            EnsureVanillaBackUpExists();

			// Create Backup file
			Backup();
		}

		/// <summary>
		/// Destructor to call dispose
		/// </summary>
		~SingleFileAction()
		{
			Dispose();
		}

		/// <summary>
		/// Rollback the instance
		/// </summary>
		public virtual void Rollback()
		{
			Logger.WriteVerbose($"Roll back result of `{GetType().Name}`");

			if (BackupPath != null)
			{
				// TODO: think of failed rollback. As it might fail, and backups would get removed in disposed.
				Logger.WriteDebug($"Replacing file `{FilePath}` with its back up `{BackupPath}`.");
				FileManager.Copy(BackupPath, FilePath, true);
			}
			else if (FileManager.FileExists(FilePath))
			{
				Logger.WriteDebug($"Remove action result `{FilePath}`.");
				FileManager.Delete(FilePath);
			}
		}

		/// <summary>
		/// Does Back up of the file
		/// </summary>
		public void Backup()
		{
			if (FileManager.FileExists(FilePath))
			{
                Logger.WriteVerbose($"Create back up for `{FilePath}`");
                BackupPath = GetNewBackUpFileName();

				if (!FileManager.FileExists(BackupPath))
				{
					Logger.WriteDebug($"Back up file `{BackupPath}` does not exists. Creating temporary back up file for `{FilePath}`.");
                    FileManager.Copy(FilePath, BackupPath);
				}
			}
		}

		/// <summary>
		/// Verify that back up file for vanilla instance is exists
		/// </summary>
		public void EnsureVanillaBackUpExists()
		{
			if (FileManager.FileExists(FilePath))
			{
				string vanillaFilePath = IshFilePath.VanillaPath;
				if (!FileManager.FileExists(vanillaFilePath))
				{
					Logger.WriteDebug($"Create vanilla back up of file `{GetType().Name}`");
					FileManager.EnsureDirectoryExists(Path.GetDirectoryName(vanillaFilePath));
					FileManager.Copy(FilePath, vanillaFilePath);
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (BackupPath != null)
			{
				if (FileManager.FileExists(BackupPath))
				{
					Logger.WriteDebug($"Remove temporary backup file `{BackupPath}`");
					FileManager.Delete(BackupPath);
				}

				BackupPath = null;
			}
		}

		#region private methods

		/// <summary>
		/// Returns back up file path
		/// </summary>
		/// <returns>Path to backup file</returns>
		private string GetNewBackUpFileName()
		{
			return String.Concat(FilePath, BackUpFileExtension);
		}

		#endregion private methods
	}
}
