using System.Runtime.InteropServices;
using System.Reflection;

namespace TeardownM;

public class Log {
    /******************************************/
    /************** DLL Imports ***************/
    /******************************************/
    [DllImport("sledge_core.dll")]
    private static extern void _WriteLog(ELogType eLogtype, string cMsg);
    
    /******************************************/
    /*************** Variables ****************/
    /******************************************/
    private enum ELogType {
        General = 4,
        Warning = 5,
        Error = 6,
        Verbose = 7
    }

    /******************************************/
    /*************** Functions ****************/
    /******************************************/
    private static void _Log(ELogType eType, Assembly aCaller, string sFormat, params object[] oArgs) {
        string sCallingAssembly = "unknown";
        string? sAssemblyName = aCaller.GetName().Name;
        if (aCaller != null && sAssemblyName != null)
            sCallingAssembly = sAssemblyName;

        string sMsg;
        try {
            sMsg = string.Format(sFormat, oArgs);
        } catch (Exception e) {
            Error("Logger error while formatting: {0} \n{1}", e.Message, Environment.StackTrace);
            return;
        }

        _WriteLog(eType, "[" + sCallingAssembly + "] - " + sMsg);
        // DateTime dt = DateTime.Now;
        // string sTime = dt.ToString("HH:mm:ss");

        // switch (eType) {
        //     case ELogType.General:
        //         Console.ForegroundColor = ConsoleColor.White;
        //         Console.WriteLine("[{0}] General: {1}", sTime, sMsg);
        //         break;
        //     case ELogType.Warning:
        //         Console.ForegroundColor = ConsoleColor.Yellow;
        //         Console.WriteLine("[{0}] Warning: {1}", sTime, sMsg);
        //         break;
        //     case ELogType.Error:
        //         Console.ForegroundColor = ConsoleColor.Red;
        //         Console.WriteLine("[{0}] Error: {1}", sTime, sMsg);
        //         break;
        //     case ELogType.Verbose:
        //         Console.ForegroundColor = ConsoleColor.Green;
        //         Console.WriteLine("[{0}] Verbose: {1}", sTime, sMsg);
        //         break;
        // }

        // Console.ForegroundColor = ConsoleColor.White;
    }

    public static void General(string sFormat, params object[] oArgs) { _Log(ELogType.General, Assembly.GetCallingAssembly(), sFormat, oArgs); }
    public static void Warning(string sFormat, params object[] oArgs) { _Log(ELogType.Warning, Assembly.GetCallingAssembly(), sFormat, oArgs); }
    public static void Error(string sFormat, params object[] oArgs) { _Log(ELogType.Error, Assembly.GetCallingAssembly(), sFormat, oArgs); }
    public static void Verbose(string sFormat, params object[] oArgs) { _Log(ELogType.Verbose, Assembly.GetCallingAssembly(), sFormat, oArgs); }
}