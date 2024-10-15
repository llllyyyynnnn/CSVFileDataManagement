using CSVFileDataManagement;

void WaitForCommand()
{
    try
    {
        string input = Console.ReadLine();
        string[] splitInput = input.ToLower().Split(' ');

        switch (splitInput[0])
        {
            case "set":
                switch (splitInput[1])
                {
                    case "index":
                        CSVManager.SetActiveIndex(int.Parse(splitInput[2]));
                        break;

                    case "file":
                        CSVManager.SetActiveFile(splitInput[2]);
                        break;
                }
                break;
        }
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
        WaitForCommand();
    }
}

Console.ForegroundColor = ConsoleManagement.Colors.foregroundColor;
CSVManager.Initialize(); // maybe make it a drag & drop

while (true)
{
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