using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mahou
{
    public static class NativeClipboard
    {
        const int MaxOpenClipboardTries = 20;
        const int OpenClipboardRetryDelayMs = 5;
        static readonly uint[] SupportedBackupFormats = new[] {
            (uint)uFormat.CF_TEXT,
            (uint)uFormat.CF_OEMTEXT,
            (uint)uFormat.CF_UNICODETEXT
        };
        #region DLL Imports/Constants
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsClipboardFormatAvailable(uint format);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GlobalUnlock(IntPtr hMem);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UIntPtr GlobalSize(IntPtr hMem);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint EnumClipboardFormats(uint format);
        public const uint GMEM_DDESHARE = 0x2000;
        public const uint GMEM_MOVEABLE = 0x2;
        public enum uFormat
        {
            CF_TEXT = 1,
            CF_BITMAP = 2,
            CF_SYLK = 4,
            CF_DIF = 5,
            CF_TIFF = 6,
            CF_OEMTEXT = 7,
            CF_DIB = 8,
            CF_PALETTE = 9,
            CF_PENDATA = 10,
            CF_RIFF = 11,
            CF_WAVE = 12,
            CF_UNICODETEXT = 13
        }
        #endregion
        public static bool Clear() // Clears Clipboard
        {
            if (!TryOpenClipboard())
                return false;
            try {
                return EmptyClipboard();
            } finally {
                CloseClipboard();
            }
        }
        public static string GetText() // Gets text data from clipboard
        {
            if (!IsClipboardFormatAvailable((uint)uFormat.CF_UNICODETEXT))
                return null;
            if (!TryOpenClipboard())
                return null;
            try {
                var hGlobal = GetClipboardData((uint)uFormat.CF_UNICODETEXT);
                if (hGlobal == IntPtr.Zero)
                    return null;
                var lpwcstr = GlobalLock(hGlobal);
                if (lpwcstr == IntPtr.Zero)
                    return null;
                try {
                    return Marshal.PtrToStringUni(lpwcstr);
                } finally {
                    GlobalUnlock(hGlobal);
                }
            } finally {
                CloseClipboard();
            }
        }
        public static ClipboardData GetClipboardDatas() // Gets all clipboard datas, but only text-based datas supported...
        {
            var cd = new ClipboardData()
            {
                data = new List<byte[]>(),
                format = new List<uint>()
            };
            if (!TryOpenClipboard())
                return cd;
            try {
                foreach (var fmt in SupportedBackupFormats) {
                    IntPtr pos = GetClipboardData(fmt);
                    if (pos == IntPtr.Zero)
                        continue;
                    UIntPtr length = GlobalSize(pos);
                    var rawLength = length.ToUInt64();
                    if (rawLength == 0 || rawLength > int.MaxValue)
                        continue;
                    var byteLength = (int)rawLength;
                    IntPtr gLock = GlobalLock(pos);
                    if (gLock == IntPtr.Zero)
                        continue;
                    try {
                        var data = new byte[byteLength];
                        Marshal.Copy(gLock, data, 0, byteLength);
                        cd.data.Add(data);
                        cd.format.Add(fmt);
                    } finally {
                        GlobalUnlock(pos);
                    }
                }
            } finally {
                CloseClipboard();
            }
            return cd;
        }
        public static bool RestoreData(ClipboardData datas) // Places all datas to clipboard, but only text-based datas supported...
        {
            if (datas.data == null || datas.format == null || datas.data.Count != datas.format.Count)
                return false;
            if (!TryOpenClipboard())
                return false;
            try {
                if (!EmptyClipboard())
                    return false;
                for (int i = 0; i != datas.data.Count; i++) {
                    var data = datas.data[i];
                    if (data == null || data.Length == 0)
                        continue;
                    IntPtr alloc = GlobalAlloc(GMEM_MOVEABLE | GMEM_DDESHARE, new UIntPtr((uint)data.Length));
                    if (alloc == IntPtr.Zero)
                        return false;
                    var transferred = false;
                    try {
                        var glock = GlobalLock(alloc);
                        if (glock == IntPtr.Zero)
                            return false;
                        try {
                            Marshal.Copy(data, 0, glock, data.Length);
                        } finally {
                            GlobalUnlock(alloc);
                        }
                        transferred = SetClipboardData(datas.format[i], alloc) != IntPtr.Zero;
                        if (!transferred)
                            return false;
                    } finally {
                        if (!transferred)
                            GlobalFree(alloc);
                    }
                }
                return true;
            } finally {
                CloseClipboard();
            }
        }
        static bool TryOpenClipboard()
        {
            for (int i = 0; i < MaxOpenClipboardTries; i++) {
                if (OpenClipboard(IntPtr.Zero))
                    return true;
                System.Threading.Thread.Sleep(OpenClipboardRetryDelayMs);
            }
            return false;
        }
        public struct ClipboardData // Struct of List of byte[](data) and uint(data format)
        {
            public List<byte[]> data;
            public List<uint> format;
        }
    }
}
