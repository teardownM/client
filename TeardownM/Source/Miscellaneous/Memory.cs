using System.Runtime.InteropServices;

namespace TeardownM.Miscellaneous;

public static class Memory {
    /******************************************/
    /************** DLL Imports ***************/
    /******************************************/
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

    /******************************************/
    /*************** Functions ****************/
    /******************************************/
    public static string GetLastErrorString() {
        // Get the error message but don't throw an exception
        int error = Marshal.GetLastWin32Error();
        string message = new System.ComponentModel.Win32Exception(error).Message;
        return message;
    }

    // Functions
    public static void Write(IntPtr pAddress, byte[] bData) {
        IntPtr pWritten;
        WriteProcessMemory(Teardown.pProcessHandle, pAddress, bData, bData.Length, out pWritten);

        if (pWritten == IntPtr.Zero)
            Log.Error("Failed writing {0} bytes to address {1:X}: {2}", bData.Length, pAddress.ToInt64(), GetLastErrorString());
    }

    public static byte[] Read(IntPtr pAddress, int iSize) {
        byte[] bData = new byte[iSize];
        IntPtr pRead;
        ReadProcessMemory(Teardown.pProcessHandle, pAddress, bData, iSize, out pRead);

        if (pRead == IntPtr.Zero)
            Log.Error("Failed reading {0} bytes from address {1}: {2}", iSize, pAddress.ToInt64(), GetLastErrorString());

        return bData;
    }

    public static int GetSize(IntPtr pAddress) {
        int iSize = 0;
        byte[] bData = new byte[1];
        IntPtr pRead;
        do {
            ReadProcessMemory(Teardown.pProcessHandle, pAddress + iSize, bData, 1, out pRead);
            iSize+=2;
        } while (bData[0] != 0);

        return iSize;
    }

    public static void DumpBytes(IntPtr pAddress, int iSize) {
        byte[] bData = Read(pAddress, iSize);
        string sData = "";
        string sChar = "";
        for (int i = 0; i < iSize; i++) {
            sData += bData[i].ToString("X2") + " ";
            sChar += (char)bData[i];

            if (i % 16 == 15) {
                if (sChar.All(c => c >= 32 && c <= 126))
                    Log.General("{0}\t{1}", sData, sChar);
                else
                    sChar = "";

                sData = "";
            }
        }

        if (sChar != "" && iSize < 16)
            Log.General("{0}\t{1}", sData, sChar);
    }
}