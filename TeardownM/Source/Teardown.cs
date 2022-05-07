using System.Diagnostics;

public static class Teardown {
    public static int iProcessID;
    public static IntPtr pProcessHandle;
    public static IntPtr pGameBase;
    public static IntPtr pHwnd;

    public static readonly Dictionary<string, int> Offsets = new Dictionary<string, int>() {
        { "UiReload", 0xAD98 },
        { "DebugJZ1", 0x3C8AD },
        { "DebugJZ2", 0xC716 },
        { "Version", 0x338DFC },
    };

    public static string sGamePath = "";
    public static string sGameVersion = "";

    public static void EnableDebugMenu() {
        Memory.Write(pGameBase + Offsets["DebugJZ1"], new byte[] { 0x90, 0x90 });
        Memory.Write(pGameBase + Offsets["DebugJZ2"], new byte[] { 0x90, 0x90 });
    }

    public static void DisableDebugMenu() {
        Memory.Write(pGameBase + Offsets["DebugJZ1"], new byte[] { 0x74, 0x24 });
        Memory.Write(pGameBase + Offsets["DebugJZ2"], new byte[] { 0x74, 0x18 });
    }

    public static bool Initialize() {
        // TeardownConsole.Initialize();

        // Make sure the offsets are correct
        Thread tCheckOffsets = new Thread(() => {
            Thread.Sleep(1000); // Wait for game to load

            foreach (KeyValuePair<string, int> iOffset in Offsets) {
                byte[] bData = Memory.Read(pGameBase + iOffset.Value, 2);
                Log.General("Offset {0} = {1:X}", iOffset.Key, BitConverter.ToUInt16(bData, 0));
                if (bData.Length == 0) {
                    Log.Error("Failed reading offset {0}. If you experience any issues, please be patient and wait for an update.", iOffset.Key);
                    break;
                }
            }

            Log.General("TeardownM Fully Loaded!");
        });
        
        tCheckOffsets.Start();

        GetProcessInfo();
        return true;
    }

    public static void GetProcessInfo() {
        Process pTeardown = Process.GetCurrentProcess();
        sGamePath = Directory.GetCurrentDirectory();
        pProcessHandle = pTeardown.Handle;
        iProcessID = pTeardown.Id;
        pGameBase = pTeardown.MainModule!.BaseAddress;
        pHwnd = pTeardown.MainWindowHandle;
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