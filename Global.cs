// everything defined in here is not exclusive to any CSVManager instance and therefore everything is static, internal and will get shared between instances

using System.Globalization;

internal class ApplicationMessages // so we don't have to hardcode anything and also so that we can reuse messages
{
    public static string InvalidFormat = "Invalid format was detected, please try again.";
    public static string IndexOutOfRange = "Requested index exceeded allowed range.";
    public static string InvalidReturnString = "invalid";
    public static string InvalidWindowSize = "The console window is too small to show the output! Resize the window and press any key.";
    public static string NoEntriesFound = "No entries were found, create one using the 'add column' command, which will allow you to create one using the contacts template.";
    public static string FileAccessFailed = "The file is either in use or the application does not have the permissions to read / write to this location";

    public static string YesString = "yes";
    public static string YesToContinue = $"('{YesString}' to continue)";
    public static string AnyKeyToContinue = $"(press any key to continue)";

    public static string IrreversibleWarning = "This action is not reversible, are you sure you want to continue?";
}
internal class Output
{
    public static char splitChar = ';'; // these are both public and static as they will only be read they will not be changed across classes (unless we want the user to specify the separator, etc which we do not support right now)
    public static char fillChar = '-';
    public static char separatorChar = '|';

    public static int columnSpacing = 2; // spacing between rows
}

internal class Colors
{
    public static ConsoleColor foregroundColor = ConsoleColor.White; // default output color
    public static ConsoleColor selectedColor = ConsoleColor.Cyan; // selectedIndex == index
    public static ConsoleColor upcomingColor = ConsoleColor.Yellow; // birthday in 10 days
    public static ConsoleColor urgentColor = ConsoleColor.Red; // birthday today
}

internal class Functions
{
    public static string[] allowedFormats = { "yyyy/MM/dd", /*"MM/dd"*/ }; // we can allow multiple date formats like this, although we will only allow yyyy/mm/dd

    public static bool ValidateDateString(string inputDate)
    {
        DateTime parsedDate;
        return DateTime.TryParseExact(inputDate, allowedFormats, null, DateTimeStyles.None, out parsedDate); // if tryparse succeeds, the given format was valid and we will return true
    }

    public static bool UpcomingDate(string inputDate, int inDays) // will verify the inputDates format just like above, check if the date is within the days given in the int and return true or false
    {
        DateTime inputDateTime;
        if (!DateTime.TryParseExact(inputDate, allowedFormats, null, DateTimeStyles.None, out inputDateTime)) // calling the above function would imply an extra DateTime reference, not necessary and we can just do it here
            return false;

        int currentYear = DateTime.Now.Year;
        DateTime targetDate = new DateTime(currentYear, inputDateTime.Month, inputDateTime.Day);
        DateTime today = DateTime.Now.Date;
        TimeSpan difference = targetDate - today; // compare the 2 dates excluding year

        return difference.TotalDays >= 0 && difference.TotalDays <= inDays;
    }

    public static bool IsDateToday(string inputDate) // works just like above, except the difference is today and not in x days
    {
        DateTime parsedDate;

        if (DateTime.TryParseExact(inputDate, allowedFormats, null, DateTimeStyles.None, out parsedDate))
        {
            DateTime today = DateTime.Now.Date;
            return today.Month == parsedDate.Month && today.Day == parsedDate.Day; // check if the month & day of today and compare it to the input, ignoring the year input
        }
        else
            return false;
    }

    public static bool UserVerification()
    {
        bool userInput = false;
        try
        {
            userInput = Console.ReadLine() == ApplicationMessages.YesString;
        }
        catch (Exception ex) { // if the input was invalid, ignore as the user has probably input a null / empty string (whatever the case is, it was not 'yes' and we shouldnt continue)
            return false;
        }

        return userInput; // if input was valid and it still wasn't 'yes', we'll get false otherwise true
    }
}