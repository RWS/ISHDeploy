using System;
using System.Reflection;

namespace InfoShare.Deployment.Core.Commands
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
