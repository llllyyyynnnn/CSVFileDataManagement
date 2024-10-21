using CSVFileDataManagement;

void WaitForCommand()
{
    try
    {
        string input = Console.ReadLine();
        string inputToLower = input.ToLower();
        string[] splitInput = input.Split(' ');
        string[] splitInputToLower = inputToLower.Split(' ');
        

        switch (splitInputToLower[0])
        {
            case "set":
                switch (splitInputToLower[1])
                {
                    case "index":
                        CSVManager.SetActiveIndex(int.Parse(splitInput[2]));
                        break;

                    case "file":
                        CSVManager.SetActiveFile(splitInput[2]);
                        break;
                }
                break;
            case "add":
                switch (splitInputToLower[1])
                {
                    case "row":
                        CSVManager.AddRow();
                        break;
                }
                break;

            case "modify":
                switch (splitInputToLower[1])
                {
                    case "row":
                        int commandLength = splitInputToLower[0].Length + splitInputToLower[1].Length + splitInputToLower[2].Length + 3;
                        CSVManager.ModifyRowData(splitInputToLower[2], input.Substring(commandLength, input.Length - commandLength));
                        break;
                }
                break;
            case "sort":
                if (splitInputToLower[1] == "alphabetical")
                    CSVManager.SortColumnByRow(splitInputToLower[2]);
                break;
            case "help":
                Console.WriteLine("set index *");
                Console.WriteLine("set file path.csv");
                Console.WriteLine("add row");
                Console.WriteLine("modify row fieldName newValue");
                break;
        }
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
        WaitForCommand();
    }
}

while (true)
{
    Console.ForegroundColor = ConsoleManagement.Colors.foregroundColor;
    Console.Clear();

    if (CSVManager.activeFileName == string.Empty)
    {
        Console.WriteLine("Enter the name of the csv file you would like to read (*.csv)");
        CSVManager.SetActiveFile(Console.ReadLine());
        CSVManager.ReadActiveFile();
    }
    else
    {
        CSVManager.PrintData();
        WaitForCommand();
    }
}