using System;
using System.Text;

namespace XopusLicense
{
    /// <summary>
    /// Summary description for License.
    /// </summary>
    public class LicenseCheckRequest
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
        public bool isDuplicateKey;
        public string type;
        public bool expired;
        public bool cancelled;
        public int revision;
        public string domain;
        public string userBrowser;

        public string parseError;

        public int userId = 0;
        public int licenseFileId = 0;
        public int licenseId = 0;
        public int disabled = 0;

        public LicenseCheckRequest(string inputString, string userBrowser)
        {
            this.userBrowser = userBrowser;

            // Get the individual parts of the input string.
            parseInputString(inputString);
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
                    throw new LicenseException("Unknown license header: " + licenseHeaderString + " != " + LICENSE_HEADER);
                offset += LICENSE_HEADER_LENGTH;

                // Version (01)
                string licenseVersionString = input.Substring(offset, LICENSE_VERSION_LENGTH);
                if (!licenseVersionString.Equals(LICENSE_VERSION))
                    throw new LicenseException("Unsupported license version: " + licenseVersionString + " != " + LICENSE_VERSION);
                offset += LICENSE_VERSION_LENGTH;

                // File key
                string licenseFileKeyString = input.Substring(offset, LICENSE_FILE_KEY_LENGTH);
                licenseFileKey = ByteArrayUtils.hexStringToByteArray(licenseFileKeyString);
                offset += LICENSE_FILE_KEY_LENGTH;

                // Revision
                string revisionString = input.Substring(offset, REVISION_LENGTH);
                revision = Int32.Parse(revisionString, System.Globalization.NumberStyles.HexNumber);
                offset += REVISION_LENGTH;

                // User key
                string userKeyString = input.Substring(offset, USER_KEY_LENGTH);
                byte[] userKeyBytes = ByteArrayUtils.hexStringToByteArray(userKeyString);
                userKey = ByteArrayUtils.byteArrayToLong(userKeyBytes);
                offset += USER_KEY_LENGTH;

                // Verify User key
                string verifyUserKeyString = input.Substring(offset, VERIFY_USER_KEY_LENGTH);
                verifyUserKey = verifyUserKeyString == "1";
                offset += VERIFY_USER_KEY_LENGTH;

                // Domain
                string domainString = input.Substring(offset);
                byte[] domainBytes = ByteArrayUtils.hexStringToByteArray(domainString);
                domain = (new ASCIIEncoding()).GetString(domainBytes);
            }
            else
            {
                throw new LicenseException("License too short: " + input.Length + " < " + totalLength);
            }
        }
    }
}
