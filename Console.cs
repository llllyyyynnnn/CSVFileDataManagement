using System.Globalization;

internal class CSVApplication
{
    internal class Colors
    {
        public static ConsoleColor foregroundColor = ConsoleColor.White;
        public static ConsoleColor selectedColor = ConsoleColor.Cyan;
        public static ConsoleColor upcomingColor = ConsoleColor.Yellow;
        public static ConsoleColor urgentColor = ConsoleColor.Red;
    }

    internal class Functions
    {
        public static string[] allowedFormats = { "yyyy/MM/dd", "MM/dd" };

        public static bool ValidateDateString(string inputDate)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(inputDate, allowedFormats, null, DateTimeStyles.None, out parsedDate);
        }

        public static bool UpcomingDate(string inputDate, int inDays)
        {
            DateTime inputDateTime;
            if (!DateTime.TryParseExact(inputDate, allowedFormats, null, System.Globalization.DateTimeStyles.None, out inputDateTime))
                return false;

            int currentYear = DateTime.Now.Year;
            DateTime targetDate = new DateTime(currentYear, inputDateTime.Month, inputDateTime.Day);
            DateTime today = DateTime.Now.Date;
            TimeSpan difference = targetDate - today;

            return difference.TotalDays >= 0 && difference.TotalDays <= inDays;
        }

        public static bool IsDateToday(string inputDate)
        {
            DateTime parsedDate;

            if (DateTime.TryParseExact(inputDate, allowedFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                DateTime today = DateTime.Now.Date;
                return today.Month == parsedDate.Month && today.Day == parsedDate.Day;
            }
            else
                return false;
        }
    }
}