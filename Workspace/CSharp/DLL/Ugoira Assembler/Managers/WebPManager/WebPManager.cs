using Newtonsoft.Json.Linq;
using Pixiv.Utilities.Ugoira.NativeCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class WebPManager
    {
        private int ugoiraPosition = -1;
        private int ugoiraCount = -1;

        private string ugoiraUserName = string.Empty;

        private string ugoiraPath = null;
        private string ugoiraPathBackup = null;

        private string ugoiraTitle = null;
        private string ugoiraTitleBackup = null;

        private List<string> cwebpArgs = null;
        private JArray frames = null;

        private static readonly string webPExtension = "webp";

        public WebPManager(string ugoiraPath, int position, int count, ActionListener actionListener = null)
        {
            this.actionListener = actionListener;

            if (!ugoiraPath.EndsWith(FileManager.zip))
                ugoiraUserName = Encoding.UTF8.GetString(Convert.FromBase64String(Path.GetExtension(ugoiraPath).Replace(FileManager.extensionSeparator.ToString(), "")));

            ugoiraPathBackup = ugoiraPath;
            ugoiraPosition = position;
            ugoiraCount = count;

            string newInputFile = Path.Combine(Directory.GetParent(ugoiraPathBackup).FullName, FileManager.getUgoiraPixivNumber(ugoiraPathBackup));
            if (!newInputFile.Equals(ugoiraPathBackup))
            {
                if (!Directory.Exists(newInputFile))
                    Directory.Move(ugoiraPathBackup, newInputFile);
                else Directory.Delete(ugoiraPathBackup, true);
            }

            ugoiraTitle = Path.GetFileName(newInputFile);
            ugoiraTitleBackup = Path.GetFileName(ugoiraPathBackup);

            if (ugoiraUserName.Length > 0)
                ugoiraTitleBackup = Path.GetFileNameWithoutExtension(ugoiraTitleBackup);

            frames = FileManager.integrityCheck(this.ugoiraPath = newInputFile);
            cwebpArgs = FileManager.getConvertingOptions();
        }

        public void produceWebP()
        {
            if (frames != null)
            {
                string tempPath = Path.Combine(FileManager.tempProgressPath, ugoiraTitle);
                string outputPath = ugoiraUserName.Length == 0 ? FileManager.outputPath : Path.Combine(FileManager.outputPath, ugoiraUserName);

                string outputFilePath = Path.Combine(outputPath, ugoiraTitle + FileManager.extensionSeparator + webPExtension);
                string newOutputFilePath = Path.Combine(outputPath, ugoiraTitleBackup + FileManager.extensionSeparator + webPExtension);

                if (File.Exists(outputFilePath))
                    File.Delete(outputFilePath);

                if (!Directory.Exists(Directory.GetParent(outputFilePath).FullName))
                    Directory.CreateDirectory(Directory.GetParent(outputFilePath).FullName);

                convertToWebP(tempPath);
                assembleWebP(tempPath, outputFilePath);

                if (!outputFilePath.Equals(newOutputFilePath))
                {
                    if (File.Exists(newOutputFilePath))
                        File.Delete(newOutputFilePath);

                    if (File.Exists(outputFilePath))
                        File.Move(outputFilePath, newOutputFilePath);
                }

                if (!ugoiraPath.Equals(ugoiraPathBackup))
                {
                    if (Directory.Exists(ugoiraPathBackup))
                        Directory.Delete(ugoiraPathBackup);

                    if (Directory.Exists(ugoiraPath))
                        Directory.Move(ugoiraPath, ugoiraPathBackup);
                }

                Directory.Delete(tempPath, true);
            }
        }

        private void assembleWebP(string tempPath, string outputFilePath)
        {
            try
            {
                onAssemblyStarted(ugoiraPath, ugoiraPosition, ugoiraCount);

                List<string> args = new List<string>();

                foreach (JToken frame in frames)
                {
                    string frameFileName = frame[FileManager.ugokuFrameFileInfo].ToString();
                    int frameDelay = int.Parse(frame[FileManager.ugokuFrameDelayInfo].ToString());

                    frameFileName = frameFileName.Substring(0, frameFileName.LastIndexOf('.'));
                    frameFileName += FileManager.extensionSeparator + webPExtension;

                    args.Add("-frame");
                    args.Add(Path.Combine(tempPath, frameFileName));
                    args.Add("+" + frameDelay);
                }
                args.Add("-o");
                args.Add(outputFilePath);

                WebPLibraryWrapper.callWebMux(args.ToArray());

                onAssemblyPerformed(ugoiraPath, ugoiraPosition, ugoiraCount);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Console.Error.WriteLine();

                onAseemblyFailed(ugoiraPath, ugoiraPosition, ugoiraCount);
            }
        }

        private void convertToWebP(string tempPath)
        {
            try
            {
                onConversionStarted(ugoiraTitleBackup, ugoiraPosition, ugoiraCount);

                if (Directory.Exists(tempPath))
                {
                    foreach (string file in Directory.GetFiles(tempPath))
                        File.Delete(file);

                    foreach (string directory in Directory.GetDirectories(tempPath))
                        Directory.Delete(directory, true);
                }
                else Directory.CreateDirectory(tempPath);

                int count = 0;
                foreach (JToken frame in frames)
                {
                    string frameFileName = frame[FileManager.ugokuFrameFileInfo].ToString();
                    List<string> args = new List<string>();

                    args.Add(Path.Combine(ugoiraPath, frameFileName));
                    args.Add("-o");
                    args.Add(Path.Combine(tempPath, Path.GetFileNameWithoutExtension(frameFileName) + FileManager.extensionSeparator + webPExtension));
                    args.AddRange(cwebpArgs);

                    onConversionProcessing(ugoiraTitleBackup, ugoiraPosition, ugoiraCount, ++count, frames.Count);

                    WebPLibraryWrapper.callWebPEncoder(args.ToArray());
                }

                onConversionPerformed(ugoiraTitleBackup, ugoiraPosition, ugoiraCount);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Console.Error.WriteLine();
                
                onConversionFailed(ugoiraTitleBackup, ugoiraPosition, ugoiraCount);
            }
        }
    }
}
