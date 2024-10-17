using System;
using System.Data;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace CSVFileDataManagement
{
    internal class CSVManager
    {
        public static string activeFileName = string.Empty;
        private static string activeFilePath = string.Empty;
        private static Dictionary<string, int> activeFileTemplate = new Dictionary<string, int>();
        private static string[] activeFileContents = Array.Empty<string>();
        private static int activeIndex = 0;

        private static char splitChar = ';';
        private static char fillChar = '-';
        private static char spaceChar = '|';

        private static int columnSpacing = 2;
        
        public static void ClearVariables()
        {
            activeFileTemplate.Clear();
            activeFileContents = Array.Empty<string>();
            activeIndex = 0;
        }

        public static void WriteActiveContentsToFile()
        {
            string writeContent = string.Empty;

            for(int x = 0; x < activeFileContents.Length; x++)
            {
                string[] splitLine = activeFileContents[x].Split(splitChar);
                string combinedLine = string.Empty;

                for(int y = 1; y < splitLine.Length; y++)
                {
                    string addLine = splitLine[y];
                    if (y < splitLine.Length - 1)
                        addLine += splitChar;
                    combinedLine += addLine;
                }

                writeContent += combinedLine;

                if (x < activeFileContents.Length - 1)
                    writeContent += Environment.NewLine;
            }

            File.WriteAllText(activeFilePath, writeContent);
        }

        public static void CreateCSVFile(string fileName)
        {
            File.Create(activeFileName);
        }

        public static void SetActiveFile(string fileName)
        {
            activeFileName = fileName;

            if (File.Exists(activeFileName))
            {
                activeFilePath = Path.GetFullPath(activeFileName);
                Console.WriteLine($"{fileName} was found");
            }
            else
            {
                Console.WriteLine($"{fileName} was not found, would you like to create it? ('yes' to continue)");
                string userInput = Console.ReadLine();

                if (userInput == "yes")
                    CreateCSVFile(fileName);
                else
                    activeFileName = string.Empty;
            }
        }

        public static void ReadActiveFile()
        {
            if (activeFileName == string.Empty || activeFilePath == string.Empty)
                return;
            ClearVariables();

            string fileData = File.ReadAllText(activeFilePath);
            activeFileContents = fileData.Split(Environment.NewLine);
            activeFileContents[0] = $"Index{splitChar}{activeFileContents[0]}"; // assign index field

            for (int i = 1; i < activeFileContents.Length; i++)
            {
                if (activeFileContents[i].Length > 0)
                    activeFileContents[i] = $"{i}{splitChar}{activeFileContents[i]}"; // assign index
            }
            string[] splitFileTemplate = activeFileContents[0].Split(splitChar);
            for(int i = 1; i < splitFileTemplate.Length; i++)
                activeFileTemplate.Add(splitFileTemplate[i].ToLower(), i);
        }

        public static void SetActiveIndex(int index)
        {
            if (index > activeFileContents.Length - 1 || index == 0)
                Console.WriteLine("Requested index exceeded allowed range.");
            else
                activeIndex = index;
        }

        private static int GetArrayMaxLength(string[] array)
        {
            int largestStringLength = 0;

            foreach (string str in array)
                if (str.Length > largestStringLength)
                    largestStringLength = str.Length;

            return largestStringLength;
        }

        private static int GetColumnLength(string[] array, int index)
        {
            int largestStringLength = 0;

            foreach(string str in array)
            {
                string[] splitEntries = str.Split(splitChar);

                if(splitEntries.Length > index)
                    if (splitEntries[index].Length > largestStringLength)
                        largestStringLength = splitEntries[index].Length;
            }

            return largestStringLength;
        }

        public static void SortListByColumn(string fieldName)
        {

        }

        public static void AddRow()
        {
            string writeString = string.Empty;

            foreach (KeyValuePair<string, int> entry in activeFileTemplate)
            {
                string key = entry.Key;
                int index = entry.Value;

                Console.WriteLine($"Current row: {key}. Waiting for input.");
                writeString += Console.ReadLine();
                if (index != activeFileTemplate.Count)
                    writeString += splitChar;
            }

            StreamWriter writer = new StreamWriter(activeFilePath, append: true);
            writer.Write($"{Environment.NewLine}{writeString}");
            writer.Close();

            ReadActiveFile();
        }

        public static void ModifyRowData(string fieldName, string newValue)
        {
            foreach (KeyValuePair<string, int> entry in activeFileTemplate)
            {
                string key = entry.Key;
                int index = entry.Value;

                Console.WriteLine($"{key}, {index}");
            }

            string lineString = activeFileContents[activeIndex];
            string[] lineStringSplit = lineString.Split(splitChar);
            lineStringSplit[activeFileTemplate[fieldName]] = newValue;
            string lineStringModified = string.Empty;
            for (int i = 0; i < lineStringSplit.Length; i++)
            {
                lineStringModified += lineStringSplit[i];
                if(i < lineStringSplit.Length - 1)
                    lineStringModified += splitChar;
            }

            activeFileContents[activeIndex] = lineStringModified;
            WriteActiveContentsToFile();
        }

        public static void PrintData()
        {
            if (activeFileContents == null || activeFileContents.Length == 0)
                return;

            Vector2 consoleSize = new Vector2(Console.WindowWidth, Console.WindowHeight);
            string consoleFillerString = new string(fillChar, (int)consoleSize.X);
            int calculatedWidthRequired = GetArrayMaxLength(activeFileContents) + 12; // will probably not work in larger fontsizes

            if (calculatedWidthRequired > Console.WindowWidth)
            {
                Console.WriteLine("The console window is too small to show the output!");
                return;
            }

            char[] textBuffer = Array.Empty<char>();

            for (int x = 0; x < activeFileContents.Length; x++)
            {
                string str = activeFileContents[x];

                string[] splitEntries = str.Split(splitChar);
                int linePosition = Console.GetCursorPosition().Top + 1;
                int activeColumnPosition = 0;

                ConsoleColor activeColor = ConsoleManagement.Colors.foregroundColor;
                if (activeIndex == x && x != 0) // 0 = template
                    activeColor = ConsoleManagement.Colors.selectedColor;
                Console.ForegroundColor = activeColor;

                for (int y = 0; y < splitEntries.Length; y++)
                {
                    int columnWidth = GetColumnLength(activeFileContents, y);

                    Console.Write(splitEntries[y]);
                    activeColumnPosition += columnWidth + columnSpacing;
                    Console.SetCursorPosition(activeColumnPosition, Console.GetCursorPosition().Top);
                    activeColumnPosition += columnSpacing;
                    Console.Write(spaceChar);
                }

                Console.Write(Environment.NewLine);
                Console.ForegroundColor = ConsoleManagement.Colors.foregroundColor;
            }
        }
    }
}