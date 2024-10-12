using System.IO;
using System.Numerics;
using System.Reflection;

namespace CSVFileDataManagement
{
    internal class CSVManager
    {
        private static string activePath = string.Empty;
        private static string activeFilePath = string.Empty;
        private static string[] activeFileContents = Array.Empty<string>();
        
        private static char splitChar = ';';
        private static char fillChar = '-';
        private static char spaceChar = '|';

        private static int columnSpacing = 2;

        public static void Initialize()
        {
            activePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Console.WriteLine($"Active path has been set to {activePath}");
        }

        public static void SetActiveFile(string fileName)
        {
            activeFilePath = $"{activePath}/{fileName}";

            if (File.Exists(activeFilePath))
            {
                Console.WriteLine($"{activeFilePath} was found, reading");
                
                string fileData = File.ReadAllText(fileName);
                activeFileContents = fileData.Split(Environment.NewLine);
                activeFileContents[0] = $"Index{splitChar}{activeFileContents[0]}"; // assign index field
                for (int i = 1; i < activeFileContents.Length; i++)
                    activeFileContents[i] = $"{i}{splitChar}{activeFileContents[i]}"; // assign index
            }
            else
            {
                Console.WriteLine($"{activeFilePath} was not found, creating empty");
                File.Create(activeFilePath);
            }
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
                if (splitEntries[index].Length > largestStringLength)
                    largestStringLength = splitEntries[index].Length;
            }

            return largestStringLength;
        }

        public static void PrintData()
        {
            Vector2 consoleSize = new Vector2(Console.WindowWidth, Console.WindowHeight);
            string consoleFillerString = new string(fillChar, (int)consoleSize.X);
            int calculatedWidthRequired = GetArrayMaxLength(activeFileContents) + 12;

            if (calculatedWidthRequired > Console.WindowWidth)
            {
                Console.WriteLine("The console window is too small to show the output!");
                return;
            }

            foreach (string str in activeFileContents) {
                string[] splitEntries = str.Split(splitChar);
                int linePosition = Console.GetCursorPosition().Top + 1;
                int activeColumnPosition = 0;

                for (int i = 0; i < splitEntries.Length; i++)
                {
                    int columnWidth = GetColumnLength(activeFileContents, i);

                    Console.SetCursorPosition(activeColumnPosition, linePosition);
                    Console.Write(splitEntries[i]);
                    activeColumnPosition += columnWidth + columnSpacing;
                    Console.SetCursorPosition(activeColumnPosition, linePosition);
                    activeColumnPosition += columnSpacing;
                    Console.Write(spaceChar);
                }
            }

            Console.SetCursorPosition(0, Console.GetCursorPosition().Top + 1);
        }
    }
}