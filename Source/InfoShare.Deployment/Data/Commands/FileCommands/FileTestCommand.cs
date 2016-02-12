using System;
using System.Reflection;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.FileCommands
{
    public class FileTestCommand : ICommand
    {
		public FileTestCommand(ILogger logger, string filePath)
        {
			//_returnResult = returnResult;
		}

		public void Execute()
		{
			//_returnResult = true;
		}
	}
}
