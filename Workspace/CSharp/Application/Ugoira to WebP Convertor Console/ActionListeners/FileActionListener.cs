using Pixiv.Utilities.Ugoira.Assembly.Managers;
using Pixiv.Utilities.Ugoira.Convert.WebP.Console;
using System;
using System.IO;

namespace Pixiv.Utilities.Ugoira.Assembly.ActionListeners
{
    public class FileActionListener : FileManager.ActionListener
    {
        public void onDecompressionStarted(string decompressedFile, int currentPosition, int fileCount)
        {
            Console.Write("Unzipping \"" + Path.GetFileName(decompressedFile) + FileManager.zip + "\"... " + Utility.progressMessage(currentPosition, fileCount));
        }

        public void onDecompressionPerformed(string decompressedFile, int currentPosition, int fileCount)
        {
            Console.WriteLine(" Done.");

            if (currentPosition == fileCount)
                Console.WriteLine();
        }

        public void onLogFileScanningStarted()
        {
            Console.Write("Scanning Log File...");
        }

        public void onLogFileScanningPerformed(bool hasError)
        {
            if (hasError)
            {
                Console.WriteLine();

                Console.WriteLine("Oops! Program may have error.");
                Console.WriteLine("Please send screen capture and log.zip file in the temp folder to me.");

                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(" Done.");
                Console.WriteLine();
            }
        }
    }
}
