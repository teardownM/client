namespace TeardownM.Miscellaneous; 

public static class Utilities {
    public static bool WriteBytesToFile(string sFilePath, byte[] bData) {
        try {
            using FileStream fFileStream = new FileStream(sFilePath, FileMode.Truncate, FileAccess.Write);
            fFileStream.Write(bData, 0, bData.Length);
            fFileStream.Close();
        } catch (FileNotFoundException) {
            Log.Error("Error Writing to File: {0} | File Not Found", sFilePath);
            return false;
        } catch (DirectoryNotFoundException) {
            Log.Error("Error Writing to File: {0} | Directory Not Found", sFilePath);
            return false;
        } catch (PathTooLongException) {
            Log.Error("Error Writing to File: {0} | Path Too Long", sFilePath);
            return false;
        } catch (UnauthorizedAccessException) {
          Log.Error("Error Writing to File: {0} | Access Denied", sFilePath);
          return false;  
        } catch (IOException) {
            Log.Error("Error Writing to File: {0} | IO Exception", sFilePath);
            return false;
        } catch (Exception) {
            Log.Error("Error Writing to File: {0} | Unknown Exception", sFilePath);
            return false;
        }

        return true;
    }
}