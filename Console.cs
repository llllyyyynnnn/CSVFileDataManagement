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