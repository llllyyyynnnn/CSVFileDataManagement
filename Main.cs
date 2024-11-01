CSVManager manager = new CSVManager();

void CommandParser(string input = "")
{
    try
    {
        if(input == "")
            input = Console.ReadLine();
        string inputToLower = input.ToLower();
        string[] splitInput = input.Split(' ');
        string[] splitInputToLower = inputToLower.Split(' ');
        
        switch (splitInputToLower[0])
        {
            case "set":
                if (splitInputToLower[1] == "index")
                    try
                    {
                        manager.SetActiveIndex(int.Parse(splitInput[2]));
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"Invalid format. {ex}");
                    }
                break;

            case "modify":
                if (splitInputToLower[1] == "row")
                    manager.ModifyRow(splitInputToLower[2]);
                break;
        }

        switch (inputToLower) // for strings that dont require any additional inputs
        {
            case "delete column":
                manager.DeleteColumn();
                break;

            case "add column":
                manager.AddColumn();
                break;
        }
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
        CommandParser();
    }
}

void ForcedCommands()
{

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
        ForcedCommands();
        manager.PrintData();
        CommandParser();
    }
}