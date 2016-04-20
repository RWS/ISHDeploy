using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
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
            ISHDeployment deployment = arguments as ISHDeployment;
            if (deployment == null)
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
			warningMessage = null;
			var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			var cmVersion = new Version(deploymentVersion.Major, deploymentVersion.Minor, deploymentVersion.Revision); // don't count about Build version.

			Regex regex = new Regex("^\\w+\\.(?<MajorVersion>\\d+)\\.(?<MinorVersion>\\d+)\\.(?<Revision>\\d+)$");
			Version moduleVersion;
			if (regex.IsMatch(moduleName) &&
	            Version.TryParse(regex.Replace(moduleName, "${MajorVersion}.${MinorVersion}.${Revision}"),
		            out moduleVersion))
			{
				int cmp = cmVersion.CompareTo(moduleVersion);
				if (cmp != 0)
				{
					warningMessage =
						$"Module version `{moduleVersion}` does not correspond to deployment version `{deploymentVersion}`.\r\nPlease install {(cmp > 0 ? $"module {moduleName}." : "newer Content Manager version.")}";
				}
			}
	        else
	        {
				warningMessage = $"Module name `{moduleName}` has different definition than expected.";
			}

			return String.IsNullOrEmpty(warningMessage);
		}
    }
}
