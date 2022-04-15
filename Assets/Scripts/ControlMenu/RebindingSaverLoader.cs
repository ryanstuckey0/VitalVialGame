using System.IO;

namespace ViralVial.ControlMenu
{
    public static class RebindingSaverLoader
    {
        public static string RebindsFilePath => $"{UnityEngine.Application.persistentDataPath}/{Utilities.Constants.RebindsSaveFileName}";

        public static void SaveRebinds(string rebinds)
        {
            Utilities.Functions.WriteToFile(rebinds, RebindsFilePath);
        }

        public static string LoadRebinds()
        {
            if (!File.Exists(RebindsFilePath)) return null;
            string rebinds = Utilities.Functions.ReadFromFile(RebindsFilePath);
            return !string.IsNullOrEmpty(rebinds) ? rebinds : null;
        }

        public static void DeleteRebindsFile()
        {
            if (!File.Exists(RebindsFilePath)) return;
            File.Delete(RebindsFilePath);
        }
    }

}