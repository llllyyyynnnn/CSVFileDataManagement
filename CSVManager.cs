using System.Text.RegularExpressions;

/*
// project goal: no List / Dictionary usage and follow Contacts structure
*/

internal class ErrorMessages
{
    public static string InvalidFormat = "Invalid format was detected, please try again.";
    public static string IndexOutOfRange = "Requested index exceeded allowed range.";
    public static string InvalidReturnString = "invalid";
    public static string InvalidWindowSize = "The console window is too small to show the output! Resize the window and press any key.";
    public static string NoEntriesFound = "No entries were found, create one using the 'add column' command, which will allow you to create one using the contacts template.";
}

internal class CSVManager
{
    struct Contact // public strings so that we can manipulate them after creating Contact objects
    {
        public string Name;
        public string PhoneNumber;
        public string Email;
        public string BirthDay;
    }

    private Contact[] contacts = Array.Empty<Contact>(); // make sure that it isn't null, so we have something to work with
    private string[] contactRowsStr = { "Name", "PhoneNumber", "Email", "Birthday" }; // makes it easier to work with the array, as we can predefine the rows and have a reference to them
    public string activeFileName = string.Empty;
    private int activeIndex = 0;

    private static char splitChar = ';'; // these are both private and static as they will only be read inside of here and they will not be changed across classes (unless we want the user to specify the separator, etc which we do not support right now)
    private static char fillChar = '-';
    private static char separatorChar = '|';

    private static int columnSpacing = 2; // spacing between rows

    private void DeleteContactIndex(ref Contact[] array, int index) // create new array, copy all relevant objects to it except the one we are trying to delte and then assign the old array to the new one
    {
        Contact[] newArray = new Contact[array.Length - 1];
        Array.Copy(array, 0, newArray, 0, index);
        Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);

        array = newArray;
    }

    public void ClearVariables() // if we want to start over again and read a new file, we have to reset all arrays and changed variables in order to not have any conflicts between files
    {
        contacts = Array.Empty<Contact>();
        activeFileName = string.Empty;
        activeIndex = 0;
    }

    public void WriteActiveContentsToFile() // REMAKE
    {
        if (activeFileName == string.Empty)
            return;

        File.WriteAllText(activeFileName, ExportContactData());
    }

    public void DeleteColumn()
    {
        if(contacts.Length > 0) // if we try to delete and there's no objects, we'll crash
        {
            Console.WriteLine($"You are trying to delete index {activeIndex}. This action is not reversible, are you sure you want to continue? ('yes' to continue)");
            string input = Console.ReadLine();
            if(input != null && input == "yes") // make sure the input isnt null or we'll crash when trying to check the string value of it, only then can we try to delete it using the function
                DeleteContactIndex(ref contacts, activeIndex);
        }

        activeIndex = 0;
    }

    private void AddContact(string name, string phoneNumber, string email, string birthDay) // create a new contact object, a new array with +1 length so that we can add it while also keeping the previous contacts through copying the array to the new one and then assigning it to the latest free index
    {
        Contact newContact = new Contact
        {
            Name = name,
            PhoneNumber = phoneNumber,
            Email = email,
            BirthDay = birthDay
        };

        Contact[] newArray = new Contact[contacts.Length + 1];
        Array.Copy(contacts, newArray, contacts.Length);
        newArray[contacts.Length] = newContact;
        contacts = newArray;
    }

    private string ExportContactData() // writes out all of the current objects in the contacts array so that we can write the string to the file and have a 1-1 copy of it (only issue is if someone had for some reason added an extra field outside of the application, it will get lost as it is not accounted for and not intended)
    {
        if (contacts == null)
            return string.Empty; // if we don't have any contacts stored or it is for some reason null, we'll simply export nothing as there is nothing to read

        string exportString = string.Empty;

        foreach (Contact contact in contacts)
            exportString += $"{contact.Name}{splitChar}" +
                $"{contact.PhoneNumber}{splitChar}" +
                $"{contact.Email}{splitChar}" +
                $"{contact.BirthDay}{Environment.NewLine}";

        return exportString;
    }

    public void AddColumn()
    {
        string writeString = string.Empty;
        Console.WriteLine("You are now creating a new column. Please enter values for the following rows.");

        for (int i = 0; i < contactRowsStr.Length; i++) // will loop through each predefined row, force the user to use the format before letting them continue and only then will it be added as a new column (contact)
        {
            string row = contactRowsStr[i];
            bool canContinue = false;
            Console.WriteLine($"Active row {row}, please enter a value.");

            while (!canContinue)
            {

                string input = Console.ReadLine();

                if (input != null)
                {
                    if (ValidateRowInput(row, input))
                    {
                        writeString += input;
                        canContinue = true;
                    }
                    else
                        Console.WriteLine(ErrorMessages.InvalidFormat);
                }
            }

            if (i < contactRowsStr.Length - 1)
                writeString += splitChar;
        }

        string[] rows = writeString.Split(splitChar);
        AddContact(rows[0], rows[1], rows[2], rows[3]); // the same way we are doing it in ReadActiveFile, we are doing here (which is part of the consistency I am going for)
    }

    public void ModifyRow(string row)
    {
        if (activeIndex > contacts.Length)
            return;

        Console.WriteLine($"Currently modifying index {activeIndex} at '{row}' with value '{AccessContactData(ref contacts[activeIndex], row)}'. Enter the new value.");
        bool canContinue = false;
        while (!canContinue)
        {
            if (AccessContactData(ref contacts[activeIndex], row, Console.ReadLine()) != ErrorMessages.InvalidReturnString) // if we don't get "invalid" (taken from ErrorMessages, so its not hardcoded) as a return value, we can continue
                canContinue = true;
            else
                Console.WriteLine(ErrorMessages.InvalidFormat); // else, tell the user we have an invalid format and to try again
        } // the function can only be left when "invalid" isnt given, and canContinue is set to true
    }

    public void SetActiveFile(string fileName) // check if the file exists, set it as the active path or create an empty one with contacts template
    {
        if (!fileName.EndsWith(".csv")) // if the user hasn't added .csv, add it here so they don't have to continously write it
            fileName += ".csv"; // if the case is that the file doesn't end with .csv because the user tried to provide another file extension, it will simply not find the file and we won't have any issues
        activeFileName = fileName;
        

        if (File.Exists(activeFileName))
            Console.WriteLine($"{fileName} was found");
        else
        {
            Console.WriteLine($"{fileName} was not found, would you like to create it? ('yes' to continue)");
            string userInput = Console.ReadLine();

            if (userInput != null && userInput == "yes")
            {
                using (FileStream fs = File.Create(fileName)) { } // using will create the file using the given FileStream reference and dispose of it afterwards, only continuing to the next line after its been disposed off and completed the command (which means we wont get a file in use error)
                Console.WriteLine("Created empty file, use the command 'add column' to add a contact");
            }
            else
                activeFileName = string.Empty;
        }
    }

    public void ReadActiveFile() // creates all contact objects that get looped throughout the code, clears previous variables incase the file was changed (such as changes being written to the file)
    {

        if (activeFileName == string.Empty)
            return;
//        ClearVariables(); // only clear variables if we are changing file, no need to do it here as this implies it should already be all clear and ready to be used

        string fileData = File.ReadAllText(activeFileName); // assign file data to a string, split it into multiple lines that will be the individual Contact objects that will get assigned to an array with the length of the amount of lines that exist in the .csv file
        string[] fileDataLines = fileData.Split(Environment.NewLine);
        contacts = new Contact[0];

        for(int i = 0; i < fileDataLines.Length; i++) // assign relevant fields to new Contact object, then assign it based on the index to the array we created above
        {
            string[] rows = fileDataLines[i].Split(splitChar);
            if (rows.Length < 4) // if we don't have name, phone, email and birthday we will have a length less than 4, which means it's invalid and we can't continue (or we'll crash as a result of rows[x] being null)
            {
                contacts = Array.Empty<Contact>();
                return;
            }

            AddContact(rows[0], rows[1], rows[2], rows[3]); // this will keep adding +1 to the array length, copy all previous objects to the array and then add a new one to the latest index
                                                            // it's definetly less efficient than simply assinging them directly here while predefining the array length as the amount of lines the file has,
                                                            // but the goal is to have as little variation between how the functions work (for reproducibility, consistency and stability)
                                                            // so that debugging is easier as this is not a commercial product and should not be treated as such
        }
    }

    public void SetActiveIndex(int index) // do not allow out of bounds indexes, will crash any function that tries to access a contact in that range
    {
        if (index > contacts.Length - 1)
            Console.WriteLine(ErrorMessages.IndexOutOfRange);
        else
            activeIndex = index;
    }

    private bool ValidateRowInput(string row, string input) // https://regexr.com/ was used, very helpful website
    {
        switch (row.ToLower()) // using tolower to prevent case sensitivity
        {
            case "birthday":
                return CSVApplication.Functions.ValidateDateString(input); // as we are using DateTime to verify date distance, etc in other functions it only makes sense we use DateTime function to verify that the date is correct, instead of regex as this is more appropiate
            case "email":
                return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); // this regex makes sure that given string follows the email standards, such as user.name@service.tld and not something like user.name@ or user@mail without a tld
            case "phonenumber":
                return Regex.IsMatch(input, @"^(\+?\d{1,2}\s?)?(\(?\d{3}\)?[\s.-]?)?\d{3}[\s.-]?\d{4}$"); // follows international phone number standards, makes sure no invalid numbers are given (such as ones that are too long, or numbers that arent using the correct 123-45678, etc)
            case "name":
                return Regex.IsMatch(input, @"^[A-Z][a-zA-Z'-]+,\s[A-Z][a-zA-Z'-]+$"); // an issue with this is that it will prevent non english characters from being written, but makes sure we are using "Full, Name" just like all other entries in the row  
        }

        return false;
    }

    private string AccessContactData(ref Contact contact, string row, string value = "") // instead of making two functions with their own string detection and make the code a mess, we can manage it all in one place so it becomes easier to add or delete row values. it'll only write to the object if there's a value given, otherwise it'll return a string value
    { // todo: cleanup
        string currentString = string.Empty;
        bool modifyContactData = value != "";

        switch (row.ToLower()) // tolower so we don't have to worry about being case sensitive, 
        {
            case "name":
                if (modifyContactData)
                {
                    if (ValidateRowInput(row, value)) // ValidateRowInput will decide if the value is following the rows that have defined formats using regex / DateTime. If it's not defined, it'll return true always
                        contact.Name = value; // will only set the value if above allows it by returning true (meaning we follow the relevant format)
                    else
                        currentString = ErrorMessages.InvalidReturnString; // since we are already returning a string value (which if not for this, would've always been empty if we were trying to modify contact data) we can tell the previous function that the format was invalid, which can help us continue a while loop until the user puts in a valid format
                }
                else
                    currentString = contact.Name;
                break;
            case "phonenumber":
                if (modifyContactData)
                {
                    if (ValidateRowInput(row, value))
                        contact.PhoneNumber = value;
                    else
                        currentString = ErrorMessages.InvalidReturnString;
                }
                else
                    currentString = contact.PhoneNumber;
                break;
            case "email":
                if (modifyContactData)
                {
                    if (ValidateRowInput(row, value))
                        contact.Email = value;
                    else
                        currentString = ErrorMessages.InvalidReturnString;
                }
                else
                    currentString = contact.Email;
                break;
            case "birthday":
                if (modifyContactData)
                {
                    if (ValidateRowInput(row, value))
                        contact.BirthDay = value;
                    else
                        currentString = ErrorMessages.InvalidReturnString;
                }
                else
                    currentString = contact.BirthDay;
                break;
        }

        return currentString; // in the other branch, this is done more efficiently as this is trying its best to have predefined formats / layouts for the csv files, while the other one is more flexible
    }

    private int GetRowWidth(Contact[] array, string row)
    {
        int largestStringLength = 0;

        for(int i = 0; i < array.Length; i++) // a foreach loop would've worked here, but because we're using ref for the contacts object as there's a possibility we want to modify it, we can't in this specific case.
        {
            string currentString = AccessContactData(ref contacts[i], row);

            if (currentString.Length > largestStringLength)
                largestStringLength = currentString.Length;
        }

        return largestStringLength;
    }

    private bool ValidateConsoleWidth() // the try function will catch any sizing errors and wont let us continue writing messages to the console window as that would be a guaranteed out of bounds error
    {
        int calculatedWidthRequired = 0;

        foreach(string str in contactRowsStr)
        {
            int width = GetRowWidth(contacts, str);
            calculatedWidthRequired += width + columnSpacing * 2; // loop through all row widths and add it to the width required, alongside an extra spacing*2 to compensate for all the extra spaces
        }

        try
        {
            int CursorPositionLeft = Console.GetCursorPosition().Left; // save it so that we can return to the original value after the test below
            Console.SetCursorPosition(calculatedWidthRequired, Console.GetCursorPosition().Top);
            Console.SetCursorPosition(CursorPositionLeft, Console.GetCursorPosition().Top); // reset the position to 0 if the above succeeded so that we dont mess up our position for the next function
        }
        catch (Exception e)
        {
            Console.WriteLine(ErrorMessages.InvalidWindowSize);
            return false;
        }

        return true; // lets the relevant functions know it is safe to print out the text using SetCursorPosition on the X axis
    }

    private void WriteData(string printString, int columnWidth, ref int columnPosition)
    {
        Console.Write(printString); columnPosition += columnWidth + columnSpacing; // write the printString, modify the reference columnPosition so that the rest of the loop is aware of where to put the next characters, write the separator and continue
        Console.SetCursorPosition(columnPosition, Console.GetCursorPosition().Top); columnPosition += columnSpacing;
        Console.Write(separatorChar);
    }

    public void PrintData() 
    {
        if(contacts == null || contacts.Length == 0) // return error message that the user should create a column (contact) as there's nothing to output
        {
            Console.WriteLine(ErrorMessages.NoEntriesFound);
            return;
        }

        if (!ValidateConsoleWidth()) // to prevent SetCursorPosition from crashing the application
            return;

        int indexWidth = contacts.Length % 10 + 4; // width is decided by largest index by % 10 (for indexes only, the rest use the GetRowWidth method as the indexes are a visible only variable)

        for (int i = -1; i < contacts.Length; i++) 
        {
            int columnPosition = 0;

            ConsoleColor activeColor = CSVApplication.Colors.foregroundColor;
            if (activeIndex == i)
                activeColor = CSVApplication.Colors.selectedColor;
            Console.ForegroundColor = activeColor;

            string indexString = "Index";
            if(i != -1)
                indexString = i.ToString(); // if we aren't on -1, we aren't trying to show the user what the row names are so we should return the actual index here so we can print it out, helping the user select it later on
            WriteData(indexString, indexWidth, ref columnPosition);

            foreach (string row in contactRowsStr) // loops through all rows, if we are on -1 again we should show the user what the row is and only after reaching 0 should we start trying to print out data from the ContactData
            {
                string printString = string.Empty;

                if (i == -1)
                    printString = row; // show the user the current row
                else
                    printString = AccessContactData(ref contacts[i], row); // because we aren't giving the function a value as a 3rd parameter, it knows we want to read and will return the row value of the contact
                int columnWidth = GetRowWidth(contacts, row); // gets the current rows width, so we can write the data using it and stay consistent and line up with the rest of the outputs

                if(row.ToLower() == "birthday")
                {
                    
                }
                WriteData(printString, columnWidth, ref columnPosition); // we are using ref columnPosition so that the function itself can have a direct reference and modify the value of the variable, so we don't have to assign it ourselves here
            }

            Console.Write(Environment.NewLine);
            Console.ForegroundColor = CSVApplication.Colors.foregroundColor; // reset the color incase we have changed it above (selected, etc)
        }
    }
}