CSVManager manager = new CSVManager(); // we can define multiple instances of the manager and have them simultaneously watch different files, etc (although only one of them can render at a time as they're clearing the console)

void CommandParser(string input = "") // if no input was given, try to get it using Console.ReadLine();
{
    try // since commands are unpredictable and anything could happen (such as the index trying to run int.Parse on non numerical characters) i'm putting them inside of a try function to catch exceptions and be able to output them consistently
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
                    manager.SetActiveIndex(int.Parse(splitInput[2]));
                break;

            case "modify":
                if (splitInputToLower[1] == "row")
                    manager.ModifyRow(splitInputToLower[2]); // request to modify row that is given by the user at the 3rd parameter
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
            case "write":
                manager.WriteActiveContentsToFile();
                break;
            case "help":
                Console.WriteLine(
                    "set index X - sets the active index\n" +
                    "modify row X - modifies the given row\n" +
                    "delete column - deletes column at active index\n" +
                    "add column - adds a column\n" +
                    "write - writes changes to file");
                CommandParser();
                break;
        }
    }
    catch (Exception ex) { // print out the exception message and run the commandparser again to wait for a new command
        Console.WriteLine(ex.Message);
        CommandParser();
    }
}

while (true)
{
    Console.ForegroundColor = Colors.foregroundColor;
    Console.Clear(); // clear after every action, and make sure to reset the color as the functions can and will change colors for different reasons

    if (manager.activeFileName == string.Empty)
    { // set the file to refer to if there is none
        Console.WriteLine("Enter the name of the csv file you would like to read (*.csv)");
        manager.SetActiveFile(Console.ReadLine());
        manager.ReadActiveFile();
    }
    else
    { // print the data and wait for user commands
        manager.PrintData();
        CommandParser();
    }
}