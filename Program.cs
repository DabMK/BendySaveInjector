using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BendySaveInjector
{
    internal class Program
    {
        private static string gameFiles = string.Empty;

        private static void Main(string[] args)
        {
            if (!Checker.IsGameInstalled()) // Check if game is installed
            {
                Quit($"Bendy and the Dark Revival is not installed in this local disk {Path.GetPathRoot(Environment.SystemDirectory)}");
            }
            if (!Checker.WasGamePlayed()) // Check if game was ever played
            {
                Quit("You have never started a game of Bendy and the Dark Revival");
            }

            // Initialize
            gameFiles = Checker.gameFiles;

            // Load the Menu
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===============================================");
                Console.WriteLine("BENDY AND DARK REVIVAL SAVE INJECTOR");
                Console.WriteLine("===============================================");
                Console.Write(Environment.NewLine);
                Console.WriteLine("1) View Saves");
                Console.WriteLine("2) Inject data into a Save");
                Console.WriteLine("3) Inject a custom Save");
                Console.WriteLine("4) Delete a Save");
                Console.WriteLine("5) Fix Manager File (use if you don't see some of your saves anymore)");
                Console.WriteLine("6) Exit" + Environment.NewLine);
                Console.Write("> ");
                ConsoleKey input = Console.ReadKey().Key;
                Console.WriteLine(Environment.NewLine);
                switch (input)
                {
                    case ConsoleKey.D1: // View saves

                        Console.WriteLine("Saves:");
                        GetSaves(true);
                        Console.Write(Environment.NewLine + "Press any button to return to the main menu");
                        Console.ReadKey();
                        continue;
                    case ConsoleKey.D2: // Inject data into a save
                        EditSave();
                        Console.Write("! Press any button to return to the main menu...");
                        Console.ReadKey();
                        continue;
                    case ConsoleKey.D3: // Inject save
                        InjectSave();
                        Console.Write(Environment.NewLine + "Save succesfully injected! Press any button to return to the main menu...");
                        Console.ReadKey();
                        continue;
                    case ConsoleKey.D4: // Delete save
                        DeleteSave();
                        Console.Write(Environment.NewLine + "Save File succesfully deleted! Press any button to return to the main menu...");
                        Console.ReadKey();
                        continue;
                    case ConsoleKey.D5: // Fix Manager File
                        bool managerFixed = Checker.FixManager();
                        if (managerFixed) { Console.Write("Manager File fixed"); } else { Console.Write("Found no errors in Manager File"); }
                        Console.Write("! Press any button to return to the main menu...");
                        Console.ReadKey();
                        continue;
                    case ConsoleKey.D6: // Exit
                        Environment.Exit(0);
                        continue;
                    default: continue;
                }
            }
        }

        private static void EditSave()
        {
            // Set variables
            Console.WriteLine("Saves:");
            List<string> files = GetSaves(true);
            string? load = string.Empty;
            int DBSize = DB.values.Count;
            int customDBSize = DB.customValues.Count;
            Console.Write(Environment.NewLine);

            // Input save to modify
            while (string.IsNullOrWhiteSpace(load) || !int.TryParse(load, out _) || int.Parse(load) > files.Count || int.Parse(load) < 1)
            {
                Console.Write("Number of Save to Modify: ");
                load = Console.ReadLine();
            }
            int save = int.Parse(load) - 1;
            Console.Write(Environment.NewLine);

            // View save's JSON data and custom data (more complex to show and edit, requires dedicated function)
            Console.WriteLine("Editable Values:");
            for (int i = 0; i < DBSize + DB.customValues.Count; i++)
            {
                if (i < DBSize)
                {
                    Console.WriteLine($"{i + 1}) {DB.values.ElementAt(i).Key}");
                }
                else
                {
                    Console.WriteLine($"{i + 1}) {DB.customValues.ElementAt(i - DBSize).Key}");
                }
            }
            Console.WriteLine("WARNING: The last audio log must be collected manually or you could break the game, therefore the max value here is 27");
            Console.WriteLine("WARNING: The last memory must be collected manually or you could break the game, therefore the max value here is 9" + Environment.NewLine);

            // Input value to change
            string? choice = string.Empty;
            while (string.IsNullOrWhiteSpace(choice) || !int.TryParse(choice, out _) || int.Parse(choice) > DBSize + customDBSize || int.Parse(choice) < 1)
            {
                Console.Write("Value to Change: ");
                choice = Console.ReadLine();
            }
            int choiceValue = int.Parse(choice) - 1;
            Console.Write(Environment.NewLine);

            // Set JSON variables
            string saveData = File.ReadAllText(files[save]);
            JObject JSON = JObject.Parse(saveData);
            JToken token = JSON;

            // Show the value
            if (choiceValue < DBSize) // If normal value
            {
                foreach (string value in DB.values.ElementAt(choiceValue).Value.Split('-'))
                {
                    token = token[value];
                }
                Console.WriteLine($"Current \"{DB.values.ElementAt(choiceValue).Key}\" value: \"{token}\"" + Environment.NewLine);
            }
            else // If custom value
            {
                Console.WriteLine($"Current \"{DB.customValues.ElementAt(choiceValue - DBSize).Key}\" value: \"{CustomEdit.GetValue(JSON, choiceValue - DBSize)}\"" + Environment.NewLine);
            }

            // Input value to change
            string? newValue = string.Empty;
            while (string.IsNullOrWhiteSpace(newValue))
            {
                Console.Write("New Value: ");
                newValue = Console.ReadLine();
            }

            // Actually change the value
            if (choiceValue < DBSize) // If normal value
            {
                token = JSON;
                for (int i = 0; i < DB.values.ElementAt(choiceValue).Value.Split('-').Length; i++)
                {
                    string currentStep = DB.values.ElementAt(choiceValue).Value.Split('-')[i];
                    if (i == DB.values.ElementAt(choiceValue).Value.Split('-').Length - 1) // If last JSON step, then change
                    {
                        // If the value is another object different than string, treat it as whatever it is
                        if (int.TryParse(newValue, out int newValueInt))
                        {
                            token[currentStep] = newValueInt;
                        }
                        else if (bool.TryParse(newValue, out bool newValueBool))
                        {
                            token[currentStep] = newValueBool;
                        }
                        else
                        {
                            token[currentStep] = newValue;
                        }
                    }
                    else
                    {
                        token = token[currentStep];
                    }
                }
            }
            else // If custom value
            {
                if (int.TryParse(newValue, out int newInt))
                {
                    bool valueEdited = CustomEdit.EditValue(ref token, newInt, choiceValue - DBSize);
                    if (!valueEdited)
                    {
                        Console.Write(Environment.NewLine + "New value is too high");
                        return;
                    }
                }
                else
                {
                    Console.Write(Environment.NewLine + "New value must be a number");
                    return;
                }
            }

            // Rewrite the save back
            string rawData = JSON.ToString(Formatting.None);
            File.WriteAllText(files[save], rawData);
            Console.Write(Environment.NewLine + "Value was successfully changed");
        }

        private static void InjectSave()
        {
            //TODO
        }

        private static void DeleteSave()
        {
            // Set variables
            Console.WriteLine("Saves:");
            List<string> saves = GetSaves(true);
            string? deleteIndex = string.Empty;
            Console.WriteLine("WARNING: Deleting autosaves (and rarely also normal saves) may hide some of the other saves from the game menu even though they are still there. To fix this issue, this software changes the selected save to the one at the bottom of the list. If it still doesn't work, try changing the selected save manually to the desired one. Sorry for the inconvenience, this is still in alpha" + Environment.NewLine);

            // Input save to delete
            while (string.IsNullOrWhiteSpace(deleteIndex) || !int.TryParse(deleteIndex, out _) || int.Parse(deleteIndex) > saves.Count || int.Parse(deleteIndex) < 1)
            {
                Console.Write("Number of Save to Delete: ");
                deleteIndex = Console.ReadLine();
            }
            int deleteId = int.Parse(deleteIndex) - 1;
            Console.Write(Environment.NewLine);

            // Remove save prints from game files and change current game if it is this one
            string deleteFile = saves[deleteId];
            string deleteSaveName = GetSaves(false, true)[deleteId];
            string managerFile = @$"{gameFiles}\data.game";
            JObject manager = JObject.Parse(File.ReadAllText(managerFile));
            int toRemove = -1;
            int id = 0;
            foreach (JToken saveFile in manager["m_SaveFileDirectory"]["m_Values"]) // Search for save file data into the game manager file
            {
                if (saveFile["m_FileName"].ToString() == deleteSaveName)
                {
                    toRemove = id;
                    break;
                }
                id++;
            }
            if (toRemove >= 0) // If save file data present in manager file, erase it
            {
                JArray array = (JArray)manager["m_SaveFileDirectory"]["m_Values"];
                array.RemoveAt(toRemove);
                JArray array2 = (JArray)manager["m_SaveFileDirectory"]["m_Keys"];
                array2.RemoveAt(toRemove);
            }

            // Shift IDs if necessary and reset current save file
            Checker.ShiftManagerData(ref manager);
            Checker.ResetCurrentSave(ref manager);

            // Rewrite back manager file
            File.WriteAllText(managerFile, manager.ToString(Formatting.None));

            // Delete save's image and save file
            File.Delete(@$"{gameFiles[..gameFiles.LastIndexOf('\\')]}\images\{deleteSaveName}.png");
            File.Delete(deleteFile);
        }

        private static List<string> GetSaves(bool show = false, bool saveShow = false)
        {
            List<string> files = [];
            int counter = 1;
            foreach (string file in Directory.GetFiles(gameFiles))
            {
                if (file.EndsWith(".save"))
                {
                    if (show)
                    {
                        Console.WriteLine($"{counter}) {Path.GetFileName(file)[..Path.GetFileName(file).LastIndexOf('.')]} ({Checker.GetSaveTime(file)})");
                        counter++;
                    }
                    if (saveShow)
                    {
                        files.Add(Path.GetFileName(file)[..Path.GetFileName(file).LastIndexOf('.')]);
                    }
                    else
                    {
                        files.Add(file);
                    }
                }
            }
            return files;
        }


        private static void Quit(string error)
        {
            Console.Write(error);
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}