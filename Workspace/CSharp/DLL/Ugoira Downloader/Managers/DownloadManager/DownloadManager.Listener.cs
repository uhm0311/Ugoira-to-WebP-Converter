using System;

namespace Pixiv.Utilities.Ugoira.Download.Managers
{
    public partial class DownloadManager
    {
        private ConnectionListener connectionListener = null;
        private DownloadListener downloadListener = null;

        public interface ActionListener 
            : ConnectionListener, DownloadListener { }

        public interface ConnectionListener
        {
            void onPixivConnectionFailed();

            Tuple<string, string> onPixivLoginNeeded();
            void onPixivLoginStarted();
            void onPixivLoginPerformed();
            void onPixivLoginFailed();

            void onUgoiraNavigationStarted(int currentPosition, int ugoiraCount);
            void onUgoiraNavigationPerformed(int currentPosition, int ugoiraCount);
        }

        public interface DownloadListener
        {
            void onUgoiraDownloadStarted(string ugoiraZipFileName, int currentPosition, int ugoiraCount);
            void onUgoiraDownloadProcessing(string ugoiraZipFileName, int ugoiraPosition, int ugoiraCount, long downloadedBytes, long totalBytes);
            void onUgoiraDownloadPerformed(string ugoiraZipFileName, string base64UgoiraZip, int currentPosition, int ugoiraCount);
            void onUgoiraDownloadFailed(string ugoiraZipFileName, int currentPosition, int ugoiraCount);
        }

        private void onPixivConnectionFailed()
        {
            if (connectionListener != null)
                connectionListener.onPixivConnectionFailed();
        }

        private Tuple<string, string> onPixivLoginNeeded()
        {
            if (connectionListener == null)
                return new Tuple<string, string>(string.Empty, string.Empty);
            else return connectionListener.onPixivLoginNeeded();
        }

        private void onPixivLoginStarted()
        {
            if (connectionListener != null)
                connectionListener.onPixivLoginStarted();
        }

        private void onPixivLoginPerformed()
        {
            if (connectionListener != null)
                connectionListener.onPixivLoginPerformed();
        }

        private void onPixivLoginFailed()
        {
            if (connectionListener != null)
                connectionListener.onPixivLoginFailed();
        }

        private void onUgoiraNavigationStarted(int currentPosition, int ugoiraCount)
        {
            if (connectionListener != null)
                connectionListener.onUgoiraNavigationStarted(currentPosition, ugoiraCount);
        }

        private void onUgoiraNavigationPerformed(int currentPosition, int ugoiraCount)
        {
            if (connectionListener != null)
                connectionListener.onUgoiraNavigationPerformed(currentPosition, ugoiraCount);

            if (currentPosition == ugoiraCount && isLoginPerformed)
                ugoiraZipFileNames.AddRange(ugoiraZipFileNamesQueue);
        }

        private void onUgoiraDownloadStarted(string ugoiraZipFileName, int currentPosition, int ugoiraCount)
        {
            if (downloadListener != null)
                downloadListener.onUgoiraDownloadStarted(ugoiraZipFileName, currentPosition, ugoiraCount);
        }

        private void onUgoiraDownloadProcessing(string ugoiraZipFileName, int ugoiraPosition, int ugoiraCount, long downloadedBytes, long totalBytes)
        {
            if (downloadListener != null)
                downloadListener.onUgoiraDownloadProcessing(ugoiraZipFileName, ugoiraPosition, ugoiraCount, downloadedBytes, totalBytes);
        }

        private void onUgoiraDownloadPerformed(string ugoiraZipFileName, string base64UgoiraZip, int currentPosition, int ugoiraCount)
        {
            if (downloadListener != null)
                downloadListener.onUgoiraDownloadPerformed(ugoiraZipFileName, base64UgoiraZip, currentPosition, ugoiraCount);
        }

        private void onUgoiraDownloadFailed(string ugoiraZipFileName, int currentPosition, int ugoiraCount)
        {
            if (downloadListener != null)
                downloadListener.onUgoiraDownloadFailed(ugoiraZipFileName, currentPosition, ugoiraCount);
        }
    }
}
