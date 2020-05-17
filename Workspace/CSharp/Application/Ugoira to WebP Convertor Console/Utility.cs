using Pixiv.Utilities.Ugoira.Assembly.Managers;
using System;

namespace Pixiv.Utilities.Ugoira.Convert.WebP.Console
{
    public static partial class Utility
    {
        public static string terminationMessage { get { return "Press any key to continue. . ."; } }

        public static string progressMessage(long currentPosition, long count)
        {
            return string.Format("({0}/{1})", currentPosition, count);
        }

        public static void clearCurrentConsoleLine(string latestMessage)
        {
            int currentLineCursor = System.Console.CursorTop;

            System.Console.SetCursorPosition(0, currentLineCursor);
            System.Console.Write(new string(FileManager.space, System.Text.Encoding.UTF8.GetByteCount(latestMessage)));
            System.Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void printTerminationMessage(bool useReadKey = true)
        {
            System.Console.Write(terminationMessage);

            if (useReadKey && !System.Console.IsInputRedirected)
                System.Console.ReadKey();
        }

        public static string inputPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = System.Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    System.Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        System.Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                        System.Console.WriteLine();
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
