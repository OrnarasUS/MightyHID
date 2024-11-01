using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MightyHID
{
    public static class HidBrowse
    {
        public static List<string> Browse()
        {
            var info = new List<string>();

            Native.HidD_GetHidGuid(out var gHid);
            var hInfoSet = Native.SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero,
                Native.DIGCF_DEVICEINTERFACE | Native.DIGCF_PRESENT);

            var iface = new Native.DeviceInterfaceData();
            iface.Size = Marshal.SizeOf(iface);
            uint index = 0;

            while (Native.SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref gHid, index, ref iface)) {
                var path = GetPath(hInfoSet, ref iface);
                var handle = Open(path);
                if (handle != Native.INVALID_HANDLE_VALUE) {
                    info.Add(path);
                    Close(handle);
                }
                index++;
            }
            if (!Native.SetupDiDestroyDeviceInfoList(hInfoSet)) throw new Win32Exception();
            
            return info;
        }

        private static IntPtr Open(string path)
        {
            return Native.CreateFile(path,
                Native.GENERIC_READ | Native.GENERIC_WRITE,
                Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE,
                IntPtr.Zero, Native.OPEN_EXISTING, Native.FILE_FLAG_OVERLAPPED,
                IntPtr.Zero);
        }

        private static void Close(IntPtr handle)
        {
            if (!Native.CloseHandle(handle)) throw new Win32Exception();
        }

        private static string GetPath(IntPtr hInfoSet, 
            ref Native.DeviceInterfaceData iface)
        {
            var detIface = new Native.DeviceInterfaceDetailData();
            var reqSize = (uint)Marshal.SizeOf(detIface);
            detIface.Size = Marshal.SizeOf(typeof(IntPtr)) == 8 ? 8 : 5;
            var status = Native.SetupDiGetDeviceInterfaceDetail(hInfoSet,
                ref iface, ref detIface, reqSize, ref reqSize, IntPtr.Zero);

            if (!status) throw new Win32Exception();

            return detIface.DevicePath;
        }
    }
}
