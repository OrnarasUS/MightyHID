using System;
using System.Runtime.InteropServices;


namespace MightyHID
{
    internal static class Native
    {
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);


        #region kernel32.dll
        
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_WRITE = 0x2;
        public const uint FILE_SHARE_READ = 0x1;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPStr)] string strName, 
            uint nAccess, uint nShareMode, IntPtr lpSecurity, 
            uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);


        #endregion
        #region hid.dll
        public static extern void HidD_GetHidGuid(out Guid gHid);

        #endregion
        #region setupapi.dll
        
        public const int DIGCF_PRESENT = 0x02;
        public const int DIGCF_DEVICEINTERFACE = 0x10;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string DevicePath;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, 
            [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, 
            IntPtr hParent, uint nFlags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr lpDeviceInfoSet, uint nDeviceInfoData, ref Guid gClass,
            uint nIndex, ref DeviceInterfaceData oInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr lpDeviceInfoSet, ref DeviceInterfaceData oInterfaceData,
            ref DeviceInterfaceDetailData oDetailData, 
            uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize,
            IntPtr lpDeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);


        #endregion
  
    }
}
