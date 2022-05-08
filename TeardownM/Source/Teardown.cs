using System.Diagnostics;
using System.Runtime.InteropServices;
using TeardownM.Miscellaneous;

namespace TeardownM;

public static class Teardown {
    /******************************************/
    /************** DLL Imports ***************/
    /******************************************/
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("oleacc.dll", SetLastError = true)]
    static extern IntPtr GetProcessHandleFromHwnd(IntPtr hWnd);

    /******************************************/
    /*************** Variables ****************/
    /******************************************/
    public static IntPtr pProcessHandle;
    public static IntPtr pGameBase;
    public static IntPtr pHwnd;

    public static readonly Dictionary<string, int> Offsets = new Dictionary<string, int> {
        { "UiReload", 0xAD98 },
        { "DebugJZ1", 0x3C8AD },
        { "DebugJZ2", 0xC716 },
        { "Version", 0x338DFC },
    };

    public static string sGamePath = "";
    public static string sGameVersion = "";

    /******************************************/
    /*************** Functions ****************/
    /******************************************/
    public static void EnableDebugMenu() {
        Memory.Write(pGameBase + Offsets["DebugJZ1"], new byte[] { 0x90, 0x90 });
        Memory.Write(pGameBase + Offsets["DebugJZ2"], new byte[] { 0x90, 0x90 });
    }

    public static void DisableDebugMenu() {
        Memory.Write(pGameBase + Offsets["DebugJZ1"], new byte[] { 0x74, 0x24 });
        Memory.Write(pGameBase + Offsets["DebugJZ2"], new byte[] { 0x74, 0x18 });
    }

    public static bool IsFocused() {
        return GetForegroundWindow().ToInt32() == pHwnd.ToInt32();
    }

    public static bool Initialize() {
        GetProcessInfo();
        sGamePath = Directory.GetCurrentDirectory();

        // Make sure the offsets are correct
        foreach (KeyValuePair<string, int> iOffset in Offsets) {
            byte[] bData = Memory.Read(pGameBase + iOffset.Value, 2);
            if (bData.Length == 0) {
                Log.Error("Failed reading offset {0}. If you experience any issues, please be patient and wait for an update.", iOffset.Key);
                return false;
            }
        }

        return true;
    }

    public static void GetProcessInfo() {
        Process pTeardown = Process.GetCurrentProcess();
        
        pGameBase = pTeardown.MainModule!.BaseAddress;

        pHwnd = FindWindow("OpenGL", "Teardown");
        pProcessHandle = GetProcessHandleFromHwnd(pHwnd);
    }

    public static string GetGameVersion() {
        if (sGameVersion != "")
            return sGameVersion;

        byte[] bVersion = Memory.Read(pGameBase + Offsets["Version"], 5);
        sGameVersion = "";
        for (int i = 0; i < bVersion.Length; i++)
            sGameVersion += (char)bVersion[i];

        return sGameVersion;
    }
}