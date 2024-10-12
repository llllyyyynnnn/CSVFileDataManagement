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
        
        private static char splitChar = ',';
        private static char fillChar = '-';
        private static char spaceChar = '|';


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
            }
            else
            {
                Console.WriteLine($"{activeFilePath} was not found, creating empty");
                File.Create(activeFilePath);
            }
        }

        private static int GetColumnSize(int index)
        {
            int largestStringLength = 0;

            foreach(string str in activeFileContents)
            {

            }

            return 0;
        }

        public static void PrintData()
        {
            Vector2 consoleSize = new Vector2(Console.WindowWidth, Console.WindowHeight);
            string consoleFillerString = new string(fillChar, (int)consoleSize.X);

            /*
             for (int x = 0; x < activeFileContents.Count(); x++)
            {
                int linePosition = Console.GetCursorPosition().Top;
                Console.SetCursorPosition(0, linePosition + 1);

                string[] splitContents = activeFileContents[x].Split(splitChar);
                for(int y = 0; y < splitContents.Count(); y++)
                {

                }
            }
             */
        }
    }
}
