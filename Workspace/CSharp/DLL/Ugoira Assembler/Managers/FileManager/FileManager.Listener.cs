
namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class FileManager
    {
        private ActionListener actionListener = null;

        public interface ActionListener
        {
            void onDecompressionStarted(string decompressedFile, int currentPosition, int fileCount);
            void onDecompressionPerformed(string decompressedFile, int currentPosition, int fileCount);

            void onLogFileScanningStarted();
            void onLogFileScanningPerformed(bool hasError);
        }

        private void onDecompressionStarted(string decompressedFile, int currentPosition, int fileCount)
        {
            if (actionListener != null)
                actionListener.onDecompressionStarted(decompressedFile, currentPosition, fileCount);
        }

        private void onDecompressionPerformed(string decompressedFile, int currentPosition, int fileCount)
        {
            if (actionListener != null)
                actionListener.onDecompressionPerformed(decompressedFile, currentPosition, fileCount);
        }

        private void onLogFileScanningStarted()
        {
            if (actionListener != null)
                actionListener.onLogFileScanningStarted();
        }

        private void onLogFileScanningPerformed(bool hasError)
        {
            if (actionListener != null)
                actionListener.onLogFileScanningPerformed(hasError);
        }
    }
}
