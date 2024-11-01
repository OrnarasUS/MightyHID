using System;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace MightyHID
{
    public sealed class HidDev : IDisposable
    {
        private IntPtr _handle;
        private FileStream _fileStream;
        private int _bufferSize;

        public void Dispose()
        {
            if (_fileStream != null) {
                _fileStream.Close();
                _fileStream = null;
            }

            Native.CloseHandle(_handle);
        }

        public bool Open(string dev, int bufferSize)
        {
            _handle = Native.CreateFile(dev, 
                Native.GENERIC_READ | Native.GENERIC_WRITE,
                Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE,
                IntPtr.Zero, Native.OPEN_EXISTING, Native.FILE_FLAG_OVERLAPPED,
                IntPtr.Zero);
            if (_handle == Native.INVALID_HANDLE_VALUE) return false;
            var shandle = new SafeFileHandle(_handle, false);
            _bufferSize = bufferSize;
            _fileStream = new FileStream(shandle, FileAccess.ReadWrite, bufferSize, true);
            return true;
        }
        
        public void Write(byte[] data)
        {
            _fileStream.Write(data, 0, data.Length);
            _fileStream.Flush();
        }

        public byte[] Read()
        {
            var data = new byte[_bufferSize];
            _ = _fileStream.Read(data, 0, _bufferSize);
            return data;
        }
    }
}
