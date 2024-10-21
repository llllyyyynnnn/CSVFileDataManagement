using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace CSVFileDataManagement
{
    internal class ConsoleManagement
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
            public static void WriteCharPosition(ref char[] buffer, int position, string write)
            {
                char[] modifiedBuffer = new char[Math.Max(buffer.Length, position) + write.Length];

                for (int i = 0; i < buffer.Length; i++)
                    modifiedBuffer[i] = buffer[i];

                if (position > buffer.Length)
                    for (int i = buffer.Length; i < position; i++)
                        modifiedBuffer[i] = ' ';

                char[] stringToCharArray = write.ToCharArray();
                for (int i = 0; i < stringToCharArray.Length; i++)
                    modifiedBuffer[position + i] = stringToCharArray[i];

                buffer = modifiedBuffer;
            }

            public static bool UpcomingDate(string inputDate, int inDays)
            {
                DateTime inputDateTime;
                string[] allowedFormats = { "yyyy/MM/dd", "MM/dd" };

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
                string[] allowedFormats = { "yyyy/MM/dd", "MM/dd" };

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
}


/*  
                for (int i = 0; i < splitEntries.Length; i++)
                {
                    int columnWidth = GetColumnLength(activeFileContents, i);
                    ConsoleManagement.Functions.WriteCharPosition(ref textBuffer, textBuffer.Length, splitEntries[i]);
                    activeColumnPosition += columnWidth + columnSpacing;
                    ConsoleManagement.Functions.WriteCharPosition(ref textBuffer, activeColumnPosition, spaceChar.ToString());
                    activeColumnPosition += columnSpacing;
                }

                ConsoleManagement.Functions.WriteCharPosition(ref textBuffer, textBuffer.Length, "\n");
            }

            Console.WriteLine(textBuffer);
 */