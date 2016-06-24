using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

/// <summary>
/// netapi32.dll pinvoke helper class
/// copied from Trisoft\Trisoft.Setup.Common project
/// </summary>
public static class NetUtil
{
    [DllImport("netapi32.dll", CharSet = CharSet.Auto)]
    static extern int NetWkstaGetInfo(string server,
        int level,
        out IntPtr info);

    [DllImport("netapi32.dll")]
    static extern int NetApiBufferFree(IntPtr pBuf);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    class WKSTA_INFO_100
    {
        public int wki100_platform_id;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string wki100_computername;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string wki100_langroup;
        public int wki100_ver_major;
        public int wki100_ver_minor;
    }

    /// <summary>
    /// Returns the netbios domain name. e.g. global.sdl.corp is GLOBAL
    /// </summary>
    /// <returns>The netbios domain name</returns>
    public static string GetMachineNetBiosDomain()
    {
        IntPtr pBuffer = IntPtr.Zero;

        WKSTA_INFO_100 info;
        int retval = NetWkstaGetInfo(null, 100, out pBuffer);
        if (retval != 0)
            throw new Win32Exception(retval);

        info = (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
        string domainName = info.wki100_langroup;
        NetApiBufferFree(pBuffer);
        return domainName;
    }
}