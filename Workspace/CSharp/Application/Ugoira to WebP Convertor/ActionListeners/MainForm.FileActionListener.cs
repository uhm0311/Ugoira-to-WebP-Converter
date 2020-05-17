using Pixiv.Utilities.Ugoira.Assembly.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv.Utilities.Ugoira.Convert.WebP
{
    partial class MainForm : FileManager.ActionListener
    {
        public void onDecompressionStarted(string decompressedFile, int currentPosition, int fileCount)
        {
            setProgressLabelText(currentPosition - 1, "Unzipping \"" + Path.GetFileName(decompressedFile) + FileManager.zip + "\"... " + progressMessage(currentPosition, fileCount));
        }

        public void onDecompressionPerformed(string decompressedFile, int currentPosition, int fileCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Done.");
        }

        public void onLogFileScanningStarted()
        {
            setMessageLabelText("Scanning Log File...");
        }

        public void onLogFileScanningPerformed(bool hasError)
        {
            if (hasError)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("Oops! Program may have error.");
                message.AppendLine("Please send screen capture and log.zip file in the temp folder to me.");

                showErrorMessage(message.ToString());
            }
            else setMessageLabelText("Scanning Log File... Done.");
        }
    }
}
