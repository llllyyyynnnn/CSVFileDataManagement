using CSVFileDataManagement;

CSVManager manager = new CSVManager();

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
                        manager.SetActiveIndex(int.Parse(splitInput[2]));
                        break;

                    case "file":
                        manager.SetActiveFile(splitInput[2]);
                        break;
                }
                break;
            case "add":
                switch (splitInputToLower[1])
                {
                    case "column":
                        manager.AddColumn();
                        break;
                }
                break;

            case "modify":
                switch (splitInputToLower[1])
                {
                    case "row":
                        int commandLength = splitInputToLower[0].Length + splitInputToLower[1].Length + splitInputToLower[2].Length + 3;
                        manager.ModifyRowData(splitInputToLower[2], input.Substring(commandLength, input.Length - commandLength));
                        break;
                }
                break;
            case "sort":
                if (splitInputToLower[1] == "alphabetical")
                    manager.SortColumnByRow(splitInputToLower[2]);
                break;
            case "help":
                Console.WriteLine("set index *");
                Console.WriteLine("set file path.csv");
                Console.WriteLine("add row");
                Console.WriteLine("modify row fieldName newValue");
                Console.WriteLine("sort alphabetical row");
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
    Console.ForegroundColor = CSVApplication.Colors.foregroundColor;
    Console.Clear();

    if (manager.activeFileName == string.Empty)
    {
        Console.WriteLine("Enter the name of the csv file you would like to read (*.csv)");
        manager.SetActiveFile(Console.ReadLine());
        manager.ReadActiveFile();
    }
    else
    {
        manager.PrintData();
        WaitForCommand();
    }
}