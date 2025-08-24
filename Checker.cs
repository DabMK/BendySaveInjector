using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace BendySaveInjector
{
    internal class Checker
    {
        readonly private static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        readonly private static Guid localLowId = new("A520A1A4-1780-4FF6-BD18-167343C5AF16");
        public static string gameFiles = @$"{appData[..appData.LastIndexOf('\\')]}\LocalLow\Joey Drew Studios\Bendy and the Dark Revival\data";

        public static bool FixManager()
        {
            // Set Variables
            string managerFile = @$"{gameFiles}\data.game";
            JObject manager = JObject.Parse(File.ReadAllText(managerFile));

            // Shift data and Reset current save
            ShiftManagerData(ref manager);
            ResetCurrentSave(ref manager);

            // Rewrite back
            File.WriteAllText(managerFile, manager.ToString(Formatting.None));
            return true;
        }

        public static void ShiftManagerData(ref JObject manager)
        {
            // Set Variables
            JArray keys = (JArray)manager["m_SaveFileDirectory"]["m_Keys"];
            JArray values = (JArray)manager["m_SaveFileDirectory"]["m_Values"];

            // Remove lone keys or values
            if (keys.Count > values.Count)
            {
                int keysCount = keys.Count;
                for (int i = values.Count; i < keysCount; i++)
                {
                    keys.RemoveAt(values.Count);
                }
            }
            else if (values.Count > keys.Count)
            {
                int valuesCount = values.Count;
                for (int i = keys.Count; i < valuesCount; i++)
                {
                    values.RemoveAt(keys.Count);
                }
            }

            // Reset keys and IDs of values
            for (int i = 0; i < keys.Count; i++)
            {
                keys[i] = i;
                values[i]["m_FileID"] = i;
            }
        }

        public static void ResetCurrentSave(ref JObject manager)
        {
            if (((JArray)manager["m_SaveFileDirectory"]["m_Keys"]).Any()) // Check if there are saves
            {
                manager["m_CurrentSaveFileID"] = ((JArray)manager["m_SaveFileDirectory"]["m_Keys"]).Last(); // Set the last save ID as the current one
                manager["m_CurrentSaveFileName"] = ((JArray)manager["m_SaveFileDirectory"]["m_Values"]).Last()["m_FileName"]; // Set the last save name as the current one
            }
            else
            {
                manager["m_CurrentSaveFileID"] = string.Empty;
                manager["m_CurrentSaveFileName"] = string.Empty;
            }
        }

        public static string GetSaveTime(string saveFile)
        {
            long unixTime = long.Parse(JObject.Parse(File.ReadAllText(saveFile))["m_TimeData"]["m_Date"].ToString());
            return new DateTime(unixTime, DateTimeKind.Utc).ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss.fff");
        }


        public static bool IsGameInstalled()
        {
            if (Directory.Exists(gameFiles))
            {
                return true;
            }
            else
            {
                string tmpGameFiles = $@"{GetKnownFolderPath(localLowId)}\Joey Drew Studios\Bendy and the Dark Revival\data";
                if (Directory.Exists(tmpGameFiles))
                {
                    gameFiles = tmpGameFiles;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool WasGamePlayed()
        {
            return File.Exists(@$"{gameFiles}\data.game");
        }

        // System APIs
        private static string GetKnownFolderPath(Guid knownFolderId)
        {
            IntPtr pszPath = IntPtr.Zero;
            try
            {
                int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0) { return Marshal.PtrToStringAuto(pszPath); }
                throw Marshal.GetExceptionForHR(hr);
            }
            finally
            {
                if (pszPath != IntPtr.Zero) { Marshal.FreeCoTaskMem(pszPath); }
            }
        }
        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}