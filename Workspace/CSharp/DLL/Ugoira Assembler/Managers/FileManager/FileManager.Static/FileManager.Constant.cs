using System.IO;

namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class FileManager
    {
        private static readonly string programPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

        public static readonly char space = ' ';
        public static readonly char extensionSeparator = '.';
        public static readonly string zip = extensionSeparator + "zip";
        public static readonly string txt = extensionSeparator + "txt";

        public static readonly string inputPath = Path.Combine(programPath, "ugoira");
        public static readonly string outputPath = Path.Combine(programPath, "webp");

        public static readonly string ugoiraLinkPath = Path.Combine(inputPath, "link" + txt);

        public static readonly string tempProgressPath = Path.Combine(programPath, "temp");
        public static readonly string libwebpBinPath = Path.Combine(programPath, "libwebp", "bin");

        private static readonly string ugoiraInfo = "animation.json";
        private static readonly string ugokuInfo = "ugokuIllustData";
        private static readonly string ugokuFrameInfo = "frames";

        public static readonly string ugokuFrameFileInfo = "file";
        public static readonly string ugokuFrameDelayInfo = "delay";

        private static readonly string tempLogFilePath = Path.Combine(tempProgressPath, "tempLog" + txt);
        private static readonly string logFilePath = Path.Combine(tempProgressPath, "log" + txt);
        private static readonly string logFileNameWithoutExtension = Path.GetFileNameWithoutExtension(logFilePath);

        private static readonly string logFilesDirectory = Path.Combine(tempProgressPath, "logs");
        private static readonly string logZipFilePath = Path.Combine(tempProgressPath, "log" + zip);

        private static readonly string configsFilePath = Path.Combine(programPath, "configs" + txt);
    }
}
