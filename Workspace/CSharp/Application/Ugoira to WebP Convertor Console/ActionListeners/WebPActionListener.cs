using Pixiv.Utilities.Ugoira.Assembly.Managers;
using Pixiv.Utilities.Ugoira.Convert.WebP.Console;
using System;

namespace Pixiv.Utilities.Ugoira.Assembly.ActionListeners
{
    public class WebPActionListener : WebPManager.ActionListener
    {
        public void onAssemblyStarted(string ugoiraPath, int currentPosition, int ugoiraCount)
        {
            Console.Write("Assembling...");
        }

        public void onAssemblyPerformed(string ugoiraPath, int currentPosition, int ugoiraCount)
        {
            Console.WriteLine(" Done.");
            Console.WriteLine();
        }

        public void onAseemblyFailed(string ugoiraPath, int currentPosition, int ugoiraCount)
        {
            Console.WriteLine(" Failed.");
            Console.WriteLine();
        }

        public void onConversionStarted(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            Console.WriteLine("Now progress on \"" + ugoiraName + "\" " + Utility.progressMessage(currentPosition, ugoiraCount));
        }

        public void onConversionProcessing(string ugoiraName, int ugoiraPosition, int ugoiraCount, int framePosition, int frameCount)
        {
            string message = "Converting... " + Utility.progressMessage(framePosition, frameCount);
            Utility.clearCurrentConsoleLine(message + " ");

            Console.Write(message);
        }

        public void onConversionPerformed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            Console.WriteLine(" Done.");
        }

        public void onConversionFailed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            Console.WriteLine(" Failed.");
            Console.WriteLine();
        }
    }
}
