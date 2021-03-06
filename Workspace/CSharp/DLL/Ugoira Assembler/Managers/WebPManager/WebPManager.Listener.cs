﻿
namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class WebPManager
    {
        private ActionListener actionListener = null;

        public interface ActionListener
        {
            void onAssemblyStarted(string ugoiraPath, string ugoiraName, int currentPosition, int ugoiraCount);
            void onAssemblyPerformed(string ugoiraPath, string ugoiraName, int currentPosition, int ugoiraCount);
            void onAseemblyFailed(string ugoiraPath, string ugoiraName, int currentPosition, int ugoiraCount);

            void onConversionStarted(string ugoiraName, int currentPosition, int ugoiraCount);
            void onConversionProcessing(string ugoiraName, int ugoiraPosition, int ugoiraCount, int framePosition, int frameCount);
            void onConversionPerformed(string ugoiraName, int currentPosition, int ugoiraCount);
            void onConversionFailed(string ugoiraName, int currentPosition, int ugoiraCount);
        }

        private void onAssemblyStarted(string ugoiraPath, string ugoiraName, int currentPosition, int ugoiraCount)
        {
            if (actionListener != null)
                actionListener.onAssemblyStarted(ugoiraPath, ugoiraName, currentPosition, ugoiraCount);
        }

        private void onAssemblyPerformed(string ugoiraPath, string ugoiraName, int currentPosition, int ugoiraCount)
        {
            if (actionListener != null)
                actionListener.onAssemblyPerformed(ugoiraPath, ugoiraName, currentPosition, ugoiraCount);
        }

        private void onAseemblyFailed(string ugoiraPath, string ugoiraName, int currentPosition, int ugoiraCount)
        {
            if (actionListener != null)
                actionListener.onAseemblyFailed(ugoiraPath, ugoiraName, currentPosition, ugoiraCount);
        }

        private void onConversionStarted(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            if (actionListener != null)
                actionListener.onConversionStarted(ugoiraName, currentPosition, ugoiraCount);
        }

        private void onConversionProcessing(string ugoiraName, int ugoiraPosition, int ugoiraCount, int framePosition, int frameCount)
        {
            if (actionListener != null)
                actionListener.onConversionProcessing(ugoiraName, ugoiraPosition, ugoiraCount, framePosition, frameCount);
        }

        private void onConversionPerformed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            if (actionListener != null)
                actionListener.onConversionPerformed(ugoiraName, currentPosition, ugoiraCount);
        }

        private void onConversionFailed(string ugoiraName, int currentPosition, int ugoiraCount)
        {
            if (actionListener != null)
                actionListener.onConversionFailed(ugoiraName, currentPosition, ugoiraCount);
        }
    }
}
