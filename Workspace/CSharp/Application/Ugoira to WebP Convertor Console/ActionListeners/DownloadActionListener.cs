using Pixiv.Utilities.Ugoira.Assembly.Managers;
using Pixiv.Utilities.Ugoira.Convert.WebP.Console;
using Pixiv.Utilities.Ugoira.Download.Managers;
using System;
using System.IO;

namespace Pixiv.Utilities.Ugoira.Download.ActionListeners
{
    public class DownloadActionListener : DownloadManager.ActionListener
    {
        public void onPixivConnectionFailed()
        {
            Console.WriteLine("Cannot connect to pixiv. Please check your internet connection or pixiv server state.");
            Console.WriteLine();
        }

        public Tuple<string, string> onPixivLoginNeeded()
        {
            string id, pw;
            Console.WriteLine("Please log in to pixiv.");

            Console.Write("E-mail address / Pixiv ID : ");
            id = Console.ReadLine();

            Console.Write("Password : ");
            pw = Utility.inputPassword();

            return new Tuple<string, string>(id, pw);
        }

        public void onPixivLoginStarted()
        {
            Console.Write("Login...");
        }

        public void onPixivLoginPerformed()
        {
            Console.WriteLine(" Done.");
            Console.WriteLine();
        }

        public void onPixivLoginFailed()
        {
            Console.WriteLine(" Login failed. Please check your ID or password.");
            Console.WriteLine();
        }

        public void onUgoiraNavigationStarted(int currentPosition, int ugoiraCount)
        {
            Console.Write("Navigating Ugoira... " + Utility.progressMessage(currentPosition, ugoiraCount));
        }

        public void onUgoiraNavigationPerformed(int currentPosition, int ugoiraCount)
        {
            Console.WriteLine(" Done.");
            if (currentPosition == ugoiraCount)
                Console.WriteLine();
        }

        public void onUgoiraDownloadStarted(string ugoiraZipFileName, int currentPosition, int ugoiraCount)
        {
            Console.Write("Downloading \"" + ugoiraZipFileName + "\"... " + Utility.progressMessage(currentPosition, ugoiraCount));
        }

        public void onUgoiraDownloadProcessing(string ugoiraZipFileName, int ugoiraPosition, int ugoiraCount, long downloadedBytes, long totalBytes)
        {
            /*string message = Utility.progressMessage(downloadedBytes, totalBytes);
            Utility.clearCurrentConsoleLine(message + " "); // Console is invalid

            Console.Write(message);*/
        }

        public void onUgoiraDownloadPerformed(string ugoiraZipFileName, string ugoiraUserName, string base64UgoiraZip, int currentPosition, int ugoiraCount)
        {
            File.WriteAllBytes(Path.Combine(FileManager.inputPath, ugoiraZipFileName), System.Convert.FromBase64String(base64UgoiraZip));

            Console.WriteLine(" Done.");
            if (currentPosition == ugoiraCount)
                Console.WriteLine();
        }

        public void onUgoiraDownloadFailed(string ugoiraZipFileName, int currentPosition, int ugoiraCount)
        {
            Console.WriteLine(" Error.");
            if (currentPosition == ugoiraCount)
                Console.WriteLine();
        }
    }
}
