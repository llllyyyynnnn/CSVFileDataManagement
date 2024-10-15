using System.IO;
using System.Numerics;
using System.Reflection;

namespace CSVFileDataManagement
{
    internal class CSVManager
    {
        private static string activePath = string.Empty;
        public static string activeFileName = string.Empty;
        private static string activeFilePath = string.Empty;
        private static string activeFileTemplate = string.Empty;
        private static string[] activeFileContents = Array.Empty<string>();
        private static int activeIndex = 0;

        private static char splitChar = ';';
        private static char fillChar = '-';
        private static char spaceChar = '|';

        private static int columnSpacing = 2;

        public static void Initialize()
        {
            activePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Console.WriteLine($"Active path has been set to {activePath}");
        }
        
        public static void CreateCSVFile(string fileName)
        {
            
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
                {
                    activeFileName = string.Empty;
                    activeFilePath = string.Empty;
                }
            }
        }

        public static void ReadActiveFile()
        {
            if (activeFileName == string.Empty || activeFilePath == string.Empty)
                return;

            string fileData = File.ReadAllText(activeFilePath);

            /*
                         if(activeFileContents != null || activeFileContents.Length > 0)
                            activeFileContents = Array.Empty<string>();
             */

            activeFileContents = fileData.Split(Environment.NewLine);
            activeFileContents[0] = $"Index{splitChar}{activeFileContents[0]}"; // assign index field
            activeFileTemplate = activeFileContents[0];

            for (int i = 1; i < activeFileContents.Length; i++)
                activeFileContents[i] = $"{i}{splitChar}{activeFileContents[i]}"; // assign index
        }

        public static void SetActiveIndex(int index)
        {
            if (index > activeFileContents.Length - 1)
                Console.WriteLine("Exceeded largest index number.");
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

                if(splitEntries.Length - 1 > index)
                    if (splitEntries[index].Length > largestStringLength)
                        largestStringLength = splitEntries[index].Length;
            }

            return largestStringLength;
        }

        public static void AddData()
        {

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

                for (int i = 0; i < splitEntries.Length; i++)
                {
                    int columnWidth = GetColumnLength(activeFileContents, i);

                    Console.Write(splitEntries[i]);
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

/*  
                for (int i = 0; i < splitEntries.Length; i++)
                {
                    int columnWidth = GetColumnLength(activeFileContents, i);
                    ConsoleManagement.Functions.WriteCharPosition(ref textBuffer, textBuffer.Length, splitEntries[i]);
                    activeColumnPosition += columnWidth + columnSpacing;
                    ConsoleManagement.Functions.WriteCharPosition(ref textBuffer, activeColumnPosition, spaceChar.ToString());
                    activeColumnPosition += columnSpacing;
                }

                ConsoleManagement.Functions.WriteCharPosition(ref textBuffer, textBuffer.Length, "\n");
            }

            Console.WriteLine(textBuffer);
 */