using System;
using System.Management.Automation;
using ISHDeploy.Models;

namespace ISHDeploy.Validators
{
    /// <summary>
    /// Validates that deployment version corresponds to module version.
    /// </summary>
    /// <seealso cref="ValidateArgumentsAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class ValidateDeploymentVersion : ValidateArgumentsAttribute
    {
        /// <summary>
        /// The very first release version of the ISHDeploy module.
        /// </summary>
        public static readonly Version ModuleInitVersion = new Version("12.0.0");

        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <param name="arguments">The instance of the <see cref="ISHDeployment"/>.</param>
        /// <param name="engineIntrinsics">The engine intrinsics.</param>
        /// <exception cref="ValidationMetadataException">
        /// Thrown when module version does not correspond to deployment version or <paramref name="arguments"/> is not instance of <see cref="ISHDeployment"/>.
        /// </exception>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            ISHDeployment deployment;
            if ((deployment = arguments as ISHDeployment) == null)
            {
                throw new ValidationMetadataException($"Validation {nameof(ValidateDeploymentVersion)} should be only applied to parameter with type {nameof(ISHDeployment)}");
            }

            string warningMessage;

            if (!CheckDeploymentVersion(deployment.SoftwareVersion, out warningMessage))
            {
                throw new ValidationMetadataException(warningMessage);
            }
        }

        /// <summary>
        /// Checks if deployment version corresponds to module version.
        /// </summary>
        /// <param name="deploymentVersion">The deployment version.</param>
        /// <param name="warningMessage">The warning message in case versions are different.</param>
        /// <returns>True if versions correspond; otherwise False.</returns>
        public static bool CheckDeploymentVersion(Version deploymentVersion, out string warningMessage)
        {
            var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var moduleVersion = new Version(moduleName.Substring(moduleName.IndexOf('.') + 1));
            
            var cmVersion = new Version($"{deploymentVersion.Major}.{deploymentVersion.Minor}.{deploymentVersion.Revision}"); // don't count about Build version.

            if (cmVersion != moduleVersion)
            {
                warningMessage = $"Module version {moduleVersion} does not correspond to deployment version {deploymentVersion}.";

                if (cmVersion >= ModuleInitVersion)
                {
                    warningMessage += $"\r\nPlease install module {moduleName}.";
                }
                else
                {
                    warningMessage += "\r\nPlease install newer Content Manager version.";
                }

                return false;
            }

            warningMessage = string.Empty;
            return true;
        }
    }
}
