using System.Data;
using System.Numerics;
using System.Text.RegularExpressions;

internal class CSVManager
{
    public string activeFileName = string.Empty;
    private Dictionary<string, int> activeFileTemplate = new Dictionary<string, int>();
    private string[] activeFileContents = Array.Empty<string>();
    private int activeIndex = 0;

    private static char splitChar = ';';
    private static char fillChar = '-';
    private static char spaceChar = '|';

    private static int columnSpacing = 2;
    private static int GetArrayMaxLength(string[] array)
    {
        int largestStringLength = 0;

        foreach (string str in array)
            if (str.Length > largestStringLength)
                largestStringLength = str.Length;

        return largestStringLength;
    }

    private void DeleteIndexFromArray(ref string[] array, int index)
    {
        string[] newArray = new string[array.Length - 1];
        Array.Copy(array, 0, newArray, 0, index);
        Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);

        array = newArray;
    }

    private static int GetColumnLength(string[] array, int index)
    {
        int largestStringLength = 0;

        foreach (string str in array)
        {
            string[] splitEntries = str.Split(splitChar);

            if (splitEntries.Length > index)
                if (splitEntries[index].Length > largestStringLength)
                    largestStringLength = splitEntries[index].Length;
        }

        return largestStringLength;
    }

    public void ClearVariables()
    {
        activeFileTemplate.Clear();
        activeFileContents = Array.Empty<string>();
        activeIndex = 0;
    }

    public void WriteActiveContentsToFile()
    {
        if (activeFileName == string.Empty)
            return;

        string writeContent = string.Empty;

        for (int x = 0; x < activeFileContents.Length; x++)
        {
            string[] splitLine = activeFileContents[x].Split(splitChar);
            string combinedLine = string.Empty;

            for (int y = 0; y < splitLine.Length; y++)
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

        File.WriteAllText(activeFileName, writeContent);
    }

    public void CreateCSVFile(string fileName)
    {
        Console.WriteLine($"Enter the file template separated by {splitChar}.");
        string template = Console.ReadLine();

        if (template == null || template.Length == 0)
        {
            Console.WriteLine("Invalid template was given.");
            CreateCSVFile(fileName);
        }
        else
        {
            using (FileStream fs = File.Create(fileName))
            using (StreamWriter writer = new StreamWriter(fs))
                writer.Write(template);

            SetActiveFile(fileName);
        }
    }

    public void SetActiveFile(string fileName)
    {
        activeFileName = fileName;

        if (File.Exists(activeFileName))
        {
            Console.WriteLine($"{fileName} was found");
        }
        else
        {
            Console.WriteLine($"{fileName} was not found, would you like to create it? ('yes' to continue)");
            string userInput = Console.ReadLine();

            if (userInput != null && userInput == "yes")
                CreateCSVFile(fileName);
            else
                activeFileName = string.Empty;
        }
    }

    public void ReadActiveFile()
    {

        if (activeFileName == string.Empty)
            return;
        ClearVariables();

        string fileData = File.ReadAllText(activeFileName);
        activeFileContents = fileData.Split(Environment.NewLine);

        string[] splitFileTemplate = activeFileContents[0].Split(splitChar);
        for (int i = 0; i < splitFileTemplate.Length; i++)
            activeFileTemplate.Add(splitFileTemplate[i].ToLower(), i);
    }

    public void SetActiveIndex(int index)
    {
        if (index > activeFileContents.Length - 1 || index == 0)
            Console.WriteLine("Requested index exceeded allowed range.");
        else
            activeIndex = index;
    }

    private bool ValidateRowEntry(string rowName, string input)
    {
        switch (rowName)
        {
            case "birthday":
                return CSVApplication.Functions.ValidateDateString(input);

            case "email":
                return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            case "phonenumber":
                return Regex.IsMatch(input, @"^(\+?\d{1,2}\s?)?(\(?\d{3}\)?[\s.-]?)?\d{3}[\s.-]?\d{4}$");
        }

        return true;
    }

    public void SortColumnByRow(string rowName)
    {
        int fieldIndex = activeFileTemplate[rowName];
        Dictionary<int, string> rowList = new Dictionary<int, string>();

        for (int i = 1; i < activeFileContents.Length; i++)
        {
            string str = activeFileContents[i];
            string[] splitLine = str.Split(splitChar);
            string activeString = splitLine[activeFileTemplate[rowName]];

            rowList.Add(i, activeString);
        }

        int currentIndex = 1;
        foreach (var entry in rowList.OrderBy(kv => kv.Value))
        {
            int rowIndex = entry.Key;
            string sortedValue = entry.Value;

            string str = activeFileContents[rowIndex];
            string[] splitLine = str.Split(splitChar);

            splitLine[fieldIndex] = sortedValue;
            //splitLine[0] = currentIndex.ToString();
            activeFileContents[currentIndex] = string.Join(splitChar.ToString(), splitLine);

            currentIndex++;
        }

        //WriteActiveContentsToFile();
    }

    public void AddColumn()
    {
        string writeString = string.Empty;

        foreach (KeyValuePair<string, int> entry in activeFileTemplate)
        {
            string key = entry.Key;
            int index = entry.Value;

            Console.WriteLine($"Current row: {key}. Waiting for input.");

            bool validatedRow = false;
            string input = string.Empty;

            while (!validatedRow)
            {
                input = Console.ReadLine();

                if (input != null)
                    validatedRow = ValidateRowEntry(key, input);

                if (!validatedRow)
                    Console.WriteLine("Invalid value was given for the active row, please try again.");
            }

            writeString += input;
            if (index != activeFileTemplate.Count - 1)
                writeString += splitChar;
        }

        StreamWriter writer = new StreamWriter(activeFileName, append: true);
        writer.Write($"{Environment.NewLine}{writeString}");
        writer.Close();

        ReadActiveFile();
    }

    public void DeleteColumn()
    {
        Console.WriteLine($"You are trying to delete the column at index {activeIndex}. Are you sure you want to continue? (enter 'yes' to delete)");
        string answer = Console.ReadLine();

        if (answer != null && answer == "yes")
        {
            DeleteIndexFromArray(ref activeFileContents, activeIndex);

            if (activeIndex > activeFileContents.Length)
                activeIndex -= 1;
        }
    }

    public void ModifyRowData(string rowName)
    {
        string lineString = activeFileContents[activeIndex];
        string[] lineStringSplit = lineString.Split(splitChar);
        Console.WriteLine($"You are trying to modify row '{rowName}' at index {activeIndex} with value '{lineStringSplit[activeFileTemplate[rowName]]}'. Please enter the new value.");

        bool validatedInput = false;
        string input = string.Empty;
        while (!validatedInput)
        {
            input = Console.ReadLine();

            if (input != null)
                validatedInput = ValidateRowEntry(rowName, input);

            if (!validatedInput)
                Console.WriteLine("Invalid value was given for the active row, please try again.");
        }

        lineStringSplit[activeFileTemplate[rowName]] = input;
        string lineStringModified = string.Empty;
        for (int i = 0; i < lineStringSplit.Length; i++)
        {
            lineStringModified += lineStringSplit[i];
            if (i < lineStringSplit.Length - 1)
                lineStringModified += splitChar;
        }

        activeFileContents[activeIndex] = lineStringModified;
        //WriteActiveContentsToFile();
    }

    private int GetTemplateIndex(string row)
    {
        row = row.ToLower();

        if (activeFileTemplate.ContainsKey(row))
            return activeFileTemplate[row];

        return -1;
    }

    public void PrintData()
    {
        if (activeFileContents == null || activeFileContents.Length == 0)
            return;

        Vector2 consoleSize = new Vector2(Console.WindowWidth, Console.WindowHeight);
        int calculatedWidthRequired = GetArrayMaxLength(activeFileContents) + 12;

        /*
                     if (calculatedWidthRequired > Console.WindowWidth) // will probably not work in larger fontsizes
                    {
                        Console.WriteLine("The console window is too small to show the output!");
                        return;
                    }
        */


        try
        {
            Console.SetCursorPosition(calculatedWidthRequired + 10, Console.GetCursorPosition().Top);
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        }
        catch (Exception e)
        {
            Console.WriteLine("The console window is too small to show the output!");
            return;
        }

        char[] textBuffer = Array.Empty<char>();

        for (int x = 0; x < activeFileContents.Length; x++)
        {
            string startStr = $"Index{splitChar}";

            if (x > 0)
                startStr = $"{x.ToString()}{splitChar}";

            string str = $"{startStr}{activeFileContents[x]}";

            string[] splitEntries = str.Split(splitChar);
            int linePosition = Console.GetCursorPosition().Top + 1;
            int activeColumnPosition = 0;

            ConsoleColor activeColor = CSVApplication.Colors.foregroundColor;
            if (activeIndex == x && x != 0) // 0 = template
                activeColor = CSVApplication.Colors.selectedColor;
            Console.ForegroundColor = activeColor;

            for (int y = 0; y < splitEntries.Length; y++)
            {
                if (y != 0 && GetTemplateIndex("birthday") == y - 1)
                {
                    if (CSVApplication.Functions.UpcomingDate(splitEntries[y], 10))
                    {
                        if (CSVApplication.Functions.IsDateToday(splitEntries[y]))
                            Console.ForegroundColor = CSVApplication.Colors.urgentColor;
                        else
                            Console.ForegroundColor = CSVApplication.Colors.upcomingColor;
                    }
                }

                int columnWidth = 0;

                if (y == 0)
                    columnWidth = activeFileContents.Length % 10 + 4;
                else
                    columnWidth = GetColumnLength(activeFileContents, y - 1);

                Console.Write(splitEntries[y]);
                activeColumnPosition += columnWidth + columnSpacing;
                Console.SetCursorPosition(activeColumnPosition, Console.GetCursorPosition().Top);
                activeColumnPosition += columnSpacing;
                Console.ForegroundColor = activeColor;
                Console.Write(spaceChar);
            }

            Console.Write(Environment.NewLine);
            Console.ForegroundColor = CSVApplication.Colors.foregroundColor;
        }
    }
}