using System;
using System.Text;
using System.Text.RegularExpressions;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
	/// <summary>
	/// Implements file copy action
	/// </summary>
    public class FileAddHistoryEntryAction : BaseAction
    {
        /// <summary>
        /// The file path
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The text.
        /// </summary>
        private readonly string _text;

        /// <summary>
        /// Returns current date in format yyyyMMdd
        /// </summary>
        private static string CurrentDate => DateTime.Now.ToString("yyyyMMdd");

        /// <summary>
        /// The deployment name
        /// </summary>
        private readonly string _ishDeploymentName;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="path">The file path.</param>
        /// <param name="text">The text.</param>
        /// <param name="ishDeploymentName">The deployment name.</param>
        public FileAddHistoryEntryAction(ILogger logger, string path, string text, string ishDeploymentName) 
			: base(logger)
        {
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _path = path;
            _text = text;
            _ishDeploymentName = ishDeploymentName;
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
            var historyEntry = new StringBuilder();
            if (!_fileManager.FileExists(_path)) // create history file with initial record
            {
                Logger.WriteVerbose("Creating history file.");

                historyEntry.AppendLine($"# {CurrentDate}");
                historyEntry.AppendLine($"$deployment = Get-ISHDeployment -Name '{_ishDeploymentName}'");
            }
            else if (IsNewDate(_fileManager.ReadAllText(_path), CurrentDate)) // group history records by date inside the file
            {
                historyEntry.AppendLine($"{Environment.NewLine}# {CurrentDate}");
            }

		    historyEntry.AppendLine(_text);

            _fileManager.Append(_path, historyEntry.ToString());
		}


        /// <summary>
        /// Returns true if current date is same as last history date.
        /// </summary>
        /// <param name="historyContent">Whole history file content.</param>
        /// <param name="currentDate">Current date.</param>
        /// <returns>True if last date in history content is the same as current date.</returns>
        private bool IsNewDate(string historyContent, string currentDate)
        {
            var lastDate = Regex.Match(historyContent, @"[#]\s\d{8}", RegexOptions.RightToLeft);

            return !lastDate.Value.EndsWith(currentDate);
        }
    }
}
