using SledgeLib;

public class LuaModLoader
{

    public static string luaModFolder = Directory.GetCurrentDirectory() + @"\mods\teardownM";

    private static void MoveFiles()
    {
        string teardownMLuaDirectory = Environment.GetEnvironmentVariable("sledgeDir") + @"\mods\TeardownM\lua";
        foreach (string newPath in Directory.GetFiles(teardownMLuaDirectory, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(teardownMLuaDirectory, luaModFolder), true);
        }
    }

    public static void Load()
    {
        if (Directory.Exists(luaModFolder))
        {
            MoveFiles();
        }
        else
        {
            Directory.CreateDirectory(luaModFolder);
            MoveFiles();
        }
    }
}
