using System;
using System.Reflection;

namespace Trisoft.Configuration.Automation.Core.Commands
{
    public class GetVersionCommand : ICommandWithResult<Version>
    {
        public Version Execute()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
