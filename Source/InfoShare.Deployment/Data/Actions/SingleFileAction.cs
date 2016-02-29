using System;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions
{
    public abstract class SingleFileAction : BaseAction, IRestorableAction
	{
		const string BACK_UP_FILE_EXTENSION = ".back";

		protected readonly ISHFilePath IshFilePath;
		protected string BackupPath;

		protected readonly IFileManager FileManager;

	    protected string FilePath => IshFilePath.AbsolutePath;

		protected SingleFileAction(ILogger logger, ISHFilePath ishFilePath)
			: this(logger)
        {
			IshFilePath = ishFilePath;

			// Make sure Vanilla backup exists
			EnsureVanillaBackUpExists();

			// Create Backup file
			Backup();
		}

		protected SingleFileAction(ILogger logger)
			: base(logger)
		{
			FileManager = ObjectFactory.GetInstance<IFileManager>();
		}

		~SingleFileAction()
		{
			Dispose();
		}

		public virtual void Rollback()
		{
			if (this.BackupPath != null)
			{
				// TODO: think of failed rollback. As it might fail, and backups would get removed in disposed.
				FileManager.Copy(this.BackupPath, this.FilePath, true);
			}
			else if (FileManager.Exists(this.FilePath))
			{
				FileManager.Delete(this.FilePath);
			}
		}

		public void Backup()
		{
			if (FileManager.Exists(this.FilePath))
			{
				this.BackupPath = GetNewBackUpFileName();

				if (!FileManager.Exists(this.BackupPath))
				{
					FileManager.Copy(this.FilePath, this.BackupPath);
				}

				//	TODO: do we need to make this file hidden?
				//	File.SetAttributes(this.BackupPath, FileAttributes.Hidden);
			}
			else
			{
				//	Otherwise backup means removing added item
				//	So do nothing here
			}
		}

		public void EnsureVanillaBackUpExists()
		{
			if (FileManager.Exists(this.FilePath))
			{
				string vanillaFilePath = IshFilePath.VanillaPath;
				if (!FileManager.Exists(vanillaFilePath))
				{
					FileManager.Copy(this.FilePath, vanillaFilePath);
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// 
		/// </summary>
		public void Dispose()
		{
			if (this.BackupPath != null)
			{
				FileManager.Delete(this.BackupPath);
			}
		}

		#region private methods

		private string GetNewBackUpFileName()
		{
			string tmpFilePath = String.Concat(FilePath, BACK_UP_FILE_EXTENSION);

			//int i = 0;
			//while (FileManager.Exists(tmpFilePath))
			//{
			//	tmpFilePath = String.Concat(tmpFilePath, (++i).ToString());
			//}

			return tmpFilePath;
		}

		#endregion private methods

	}
}
