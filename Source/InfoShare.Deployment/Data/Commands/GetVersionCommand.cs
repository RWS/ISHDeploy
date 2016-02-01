using System;
using System.Reflection;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands
{
    public class GetVersionCommand : ICommand
    {
        private readonly Action<Version> _returnResult;

        public GetVersionCommand(Action<Version> returnResult)
        {
            _returnResult = returnResult;
        }
        
        public void Execute()
        {
            var result = Assembly.GetExecutingAssembly().GetName().Version;

            _returnResult?.Invoke(result);
        }
    }
}
