using Newtonsoft.Json.Linq;
using Pixiv.Utilities.Ugoira.NativeCode;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class WebPManager
    {
        private int ugoiraPosition = -1;
        private int ugoiraCount = -1;

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
            frames = FileManager.integrityCheck(this.ugoiraPath = newInputFile);
            cwebpArgs = FileManager.getConvertingOptions();
        }

        public void produceWebP()
        {
            if (frames != null)
            {
                string tempPath = Path.Combine(FileManager.tempProgressPath, ugoiraTitle);
                string outputFilePath = Path.Combine(FileManager.outputPath, ugoiraTitle + FileManager.extensionSeparator + webPExtension);
                string newOutputFilePath = Path.Combine(FileManager.outputPath, ugoiraTitleBackup + FileManager.extensionSeparator + webPExtension);

                if (File.Exists(outputFilePath))
                    File.Delete(outputFilePath);

                convertToWebP(tempPath);
                assembleWebP(tempPath, outputFilePath);

                if (!outputFilePath.Equals(newOutputFilePath))
                {
                    if (File.Exists(newOutputFilePath))
                        File.Delete(newOutputFilePath);
                    File.Move(outputFilePath, newOutputFilePath);
                }

                if (!ugoiraPath.Equals(ugoiraPathBackup))
                {
                    if (Directory.Exists(ugoiraPathBackup))
                        Directory.Delete(ugoiraPathBackup);
                    Directory.Move(ugoiraPath, ugoiraPathBackup);
                }
                Directory.Delete(tempPath, true);
            }
        }

        private void assembleWebP(string tempPath, string outputFilePath)
        {
            try
            {
                onAssemblyStarted(ugoiraTitleBackup, ugoiraPosition, ugoiraCount);

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

                onAssemblyPerformed(ugoiraTitleBackup, ugoiraPosition, ugoiraCount);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Console.Error.WriteLine();

                onAseemblyFailed(ugoiraTitleBackup, ugoiraPosition, ugoiraCount);
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
