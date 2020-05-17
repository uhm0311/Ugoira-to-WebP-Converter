using Pixiv.Utilities.Ugoira.Assembly.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv.Utilities.Ugoira.Convert.WebP
{
    partial class MainForm : WebPManager.ActionListener
    {
        public void onAssemblyStarted(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, "Assembling \"" + ugoiraName + "\"... " + progressMessage(currentPosition, ugoiraCount));
        }

        public void onAssemblyPerformed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Done.");
        }

        public void onAseemblyFailed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Failed.");
        }

        public void onConversionStarted(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            //setProgressLabelText(currentPosition - 1, "Now progress on \"" + ugoiraName + "\" " + progressMessage(currentPosition, ugoiraCount));
        }

        public void onConversionProcessing(string ugoiraName, int ugoiraPosition, int ugoiraCount, int framePosition, int frameCount)
        {
            setProgressLabelText(ugoiraPosition - 1, "Converting \"" + ugoiraName + "\"... " + progressMessage(framePosition, frameCount));
            setProgressBarValue(ugoiraPosition - 1, framePosition, frameCount);
        }

        public void onConversionPerformed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Done.");
        }

        public void onConversionFailed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            setProgressLabelText(currentPosition - 1, getProgressLabelText(currentPosition - 1) + " Failed.");
        }
    }
}
