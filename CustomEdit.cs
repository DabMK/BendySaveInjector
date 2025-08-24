using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BendySaveInjector
{
    internal class CustomEdit
    {
        private static string valuesName;
        private static int counter;
        private static JArray keys = [], values = [];

        public static int GetValue(JObject data, int customID)
        {
            // Set variables
            JToken token = data;
            counter = 0;
            valuesName = DB.customValuesNames[customID]; if (valuesName == string.Empty) { valuesName = "m_DataID"; }

            // Follow the right JSON path
            foreach (string value in DB.customValues.ElementAt(customID).Value.Split('-'))
            {
                token = token[value];
            }
            keys = (JArray)token["m_Keys"];
            values = (JArray)token["m_Values"];

            // Check for right IDs for each custom value
            foreach (int ID in DB.customValuesIDs[customID])
            {
                Check(ID);
            }
            return counter;
        }

        public static bool EditValue(ref JToken token, int newData, int customID)
        {
            // Check newData
            if (newData < 0 || newData > DB.customValuesIDs[customID].Count)
            {
                return false;
            }

            // Follow the right JSON path and set variables
            int oldData = GetValue((JObject)token, customID);
            valuesName = DB.customValuesNames[customID]; if (valuesName == string.Empty) { valuesName = "m_DataID"; }
            foreach (string value in DB.customValues.ElementAt(customID).Value.Split('-'))
            {
                token = token[value];
            }
            keys = (JArray)token["m_Keys"];
            values = (JArray)token["m_Values"];
            JArray refKeys = keys; JArray refValues = values;

            if (newData > oldData) // Add Items
            {
                int difference = newData - oldData;
                for (int i = 0; i < difference; i++) // Repeat add action for every item to add
                {
                    int newID = CheckRandomRemaining(customID);
                    refKeys.Add(newID);
                    refValues.Add(new JObject { [valuesName] = newID });
                }
                refKeys = [.. refKeys.Select(n => (int)n).OrderBy(n => n)]; // Sort keys
            }
            else if (newData < oldData) // Remove Items
            {
                int difference = oldData - newData;
                for (int i = 0; i < difference; i++) // Repeat remove action for every item to remove
                {
                    refKeys.RemoveAt(refKeys.Count - 1);
                    refValues.RemoveAt(refValues.Count - 1);
                }
            }
            return true;
        }


        private static void Check(int ID)
        {
            bool flag = false;
            foreach (JToken token in values)
            {
                if (int.TryParse(token[valuesName].ToString(), out int id) && id == ID)
                {
                    flag = true;
                    break;
                }
            }
            if (flag && keys.Any(k => (int)k == ID))
            {
                counter++;
            }
        }

        private static int CheckRandomRemaining(int customID)
        {
            int result = 0;
            foreach (int ID in DB.customValuesIDs[customID]) // Cycle all official IDs
            {
                if (result != 0) { return result; }
                result = ID;
                foreach (JToken token in values) // Cycle all present IDs
                {
                    if (int.TryParse(token[valuesName].ToString(), out int id) && id == ID)
                    {
                        result = 0;
                        break; // Keep the result set to 0 if ID is present
                    }
                }
            }
            return result;
        }
    }
}