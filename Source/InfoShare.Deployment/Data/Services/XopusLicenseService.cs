using System;
using System.Globalization;
using System.Text;

namespace  InfoShare.Deployment.Data.Services
{
	/// <summary>
	/// Summary description for LicenseParseException.
	/// </summary>
	public class XopusLicenseException : Exception
	{
		public XopusLicenseException(string msg) : base(msg)
		{
		}
	}

	/// <summary>
	/// Summary description for License.
	/// </summary>
	public class XopusLicenseService
    {
        const string LICENSE_HEADER = "4242";
        const string LICENSE_VERSION = "01";

        public const int KEY_LENGTH = 255;

        public const int LICENSE_HEADER_LENGTH = 4;
        public const int LICENSE_VERSION_LENGTH = 2;
        public const int LICENSE_FILE_KEY_LENGTH = KEY_LENGTH * 2;
        public const int REVISION_LENGTH = 4;
        public const int USER_KEY_LENGTH = 16;
        public const int VERIFY_USER_KEY_LENGTH = 1;

        public byte[] licenseFileKey;
        public long userKey;
        public bool verifyUserKey;
        public int revision;
        public string domain;

        public XopusLicenseService(string inputString)
        {
            // Get the individual parts of the input string.
            parseInputString(inputString);
		}


		public bool IsValid(string hostName)
		{
			return (hostName == domain);
		}

		public bool IsValid()
		{
			// Very simple check if everything was parsed correctly
			return String.IsNullOrEmpty(domain);
		}

		void parseInputString(string input)
        {
            int totalLength =
                LICENSE_HEADER_LENGTH + 
                LICENSE_VERSION_LENGTH + 
                LICENSE_FILE_KEY_LENGTH + 
                REVISION_LENGTH + 
                USER_KEY_LENGTH;

            if (input.Length >= totalLength)
            {
                int offset = 0;

                // Header (4242)
                string licenseHeaderString = input.Substring(offset, LICENSE_HEADER_LENGTH);
                if (!licenseHeaderString.Equals(LICENSE_HEADER))
                {
	                throw new XopusLicenseException("Unknown license header: " + licenseHeaderString + " != " + LICENSE_HEADER);
				}
                offset += LICENSE_HEADER_LENGTH;

                // Version (01)
                string licenseVersionString = input.Substring(offset, LICENSE_VERSION_LENGTH);
                if (!licenseVersionString.Equals(LICENSE_VERSION))
                {
	                throw new XopusLicenseException("Unsupported license version: " + licenseVersionString + " != " + LICENSE_VERSION);
                }
                offset += LICENSE_VERSION_LENGTH;

                // File key
                string licenseFileKeyString = input.Substring(offset, LICENSE_FILE_KEY_LENGTH);
                licenseFileKey = HexStringToByteArray(licenseFileKeyString);
                offset += LICENSE_FILE_KEY_LENGTH;

                // Revision
                string revisionString = input.Substring(offset, REVISION_LENGTH);
                revision = Int32.Parse(revisionString, System.Globalization.NumberStyles.HexNumber);
                offset += REVISION_LENGTH;

                // User key
                string userKeyString = input.Substring(offset, USER_KEY_LENGTH);
                byte[] userKeyBytes = HexStringToByteArray(userKeyString);
                userKey = ByteArrayToLong(userKeyBytes);
                offset += USER_KEY_LENGTH;

                // Verify User key
                string verifyUserKeyString = input.Substring(offset, VERIFY_USER_KEY_LENGTH);
                verifyUserKey = verifyUserKeyString == "1";
                offset += VERIFY_USER_KEY_LENGTH;

                // Domain
                string domainString = input.Substring(offset);
                byte[] domainBytes = HexStringToByteArray(domainString);
                domain = (new ASCIIEncoding()).GetString(domainBytes);
            }
            else
            {
                throw new XopusLicenseException("License too short: " + input.Length + " < " + totalLength);
            }
        }

		private static byte[] HexStringToByteArray(string s)
		{
			if (s.Length%2 != 0)
			{
				throw new XopusLicenseException("Only even length hex strings can be decoded to bytes: " + s);
			}

			byte[] buf = new byte[s.Length / 2];
			for (int offset = 0; offset < s.Length; offset += 2)
			{
				buf[offset / 2] = byte.Parse(s.Substring(offset, 2), NumberStyles.HexNumber);
			}

			return buf;
		}

		private static long ByteArrayToLong(byte[] buf)
		{
			long l = 0L;
			for (int i = 7; i >= 0; i--)
			{
				l <<= 8;
				l |= buf[i];
			}

			return l;
		}
	}
}
