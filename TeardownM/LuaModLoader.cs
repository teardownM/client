using SledgeLib;

public class LuaModLoader
{

    public static string teardownLuaModFolder = Directory.GetCurrentDirectory() + @"\mods\teardownM";

    private static void MoveFiles()
    {
        string localLuaDirectory = Environment.GetEnvironmentVariable("sledgeDir") + @"\mods\TeardownM\lua\dist";
        File.Copy(localLuaDirectory + "\\info.txt", teardownLuaModFolder + "\\info.txt", true);

        foreach (string filePath in Directory.GetFiles(localLuaDirectory, "*.lua"))
        {
            // Unfortunately, Teardown Lua does not allow require(). To import other files it needs #include (something only for Teardown)
            // The lines below remove the first 2 lines of file (expect for info.text) and replaces them with #include.
            // Currently we hardcode the values, meaning that some files will get an include when not necessary. In the future we can look
            // into doing this more defensive.
            string tempFile = Path.GetTempFileName();
            string[] lines = File.ReadAllLines(filePath);

            File.WriteAllText(tempFile, "#include \"vehicle.lua\"\n#include \"utils.lua\"\n\n");
            File.AppendAllLines(tempFile, lines.Skip(2).ToArray());

            File.Copy(tempFile, filePath.Replace(localLuaDirectory, teardownLuaModFolder), true);
            File.Delete(tempFile);
        }
    }

    public static void Load()
    {
        if (!Directory.Exists(teardownLuaModFolder))
            Directory.CreateDirectory(teardownLuaModFolder);

        MoveFiles();
    }
}
