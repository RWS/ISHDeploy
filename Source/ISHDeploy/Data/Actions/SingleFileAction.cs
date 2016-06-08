using System;
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
			Logger.WriteVerbose($"Roll back result of `{this.GetType().Name}`");

			if (this.BackupPath != null)
			{
				// TODO: think of failed rollback. As it might fail, and backups would get removed in disposed.
				Logger.WriteDebug($"Replacing file `{this.FilePath}` with its back up `{this.BackupPath}`.");
				FileManager.Copy(this.BackupPath, this.FilePath, true);
			}
			else if (FileManager.FileExists(this.FilePath))
			{
				Logger.WriteDebug($"Remove action result `{this.FilePath}`.");
				FileManager.Delete(this.FilePath);
			}
		}

		/// <summary>
		/// Does Back up of the file
		/// </summary>
		public void Backup()
		{
			

			if (FileManager.FileExists(this.FilePath))
			{
                Logger.WriteVerbose($"Create back up for `{this.FilePath}`");
                this.BackupPath = GetNewBackUpFileName();

				if (!FileManager.FileExists(this.BackupPath))
				{
					Logger.WriteDebug($"Back up file `{this.BackupPath}` does not exists. Creating temporary back up file for `{this.FilePath}`.");
                    FileManager.Copy(this.FilePath, this.BackupPath);
				}
			}
		}

		/// <summary>
		/// Verify that back up file for vanilla instance is exists
		/// </summary>
		public void EnsureVanillaBackUpExists()
		{
			if (FileManager.FileExists(this.FilePath))
			{
				string vanillaFilePath = IshFilePath.VanillaPath;
				if (!FileManager.FileExists(vanillaFilePath))
				{
					Logger.WriteDebug($"Create vanilla back up of file `{this.GetType().Name}`");
					FileManager.EnsureDirectoryExists(Path.GetDirectoryName(vanillaFilePath));
					FileManager.Copy(this.FilePath, vanillaFilePath);
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (this.BackupPath != null)
			{
				if (FileManager.FileExists(this.BackupPath))
				{
					Logger.WriteDebug($"Remove temporary backup file `{this.BackupPath}`");
					FileManager.Delete(this.BackupPath);
				}

				this.BackupPath = null;
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
