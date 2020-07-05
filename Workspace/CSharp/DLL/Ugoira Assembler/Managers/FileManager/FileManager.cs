using CommonUtilities.StandardStreams.Redirectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class FileManager : IDisposable
    {
        private bool hasError = false;
        private bool errorLogScanned = false;

        public FileManager(ActionListener actionListener = null)
        {
            this.actionListener = actionListener;

            if (!Directory.Exists(inputPath))
                Directory.CreateDirectory(inputPath);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (!Directory.Exists(tempProgressPath))
                Directory.CreateDirectory(tempProgressPath);

            StandardStreamRedirector.open(StandardStreamRedirector.StreamFileDescriptor.Error, tempLogFilePath);
        }

        public void Dispose()
        {
            StandardStreamRedirector.close(StandardStreamRedirector.StreamFileDescriptor.Error);

            if (!errorLogScanned)
                scanErrorLog();

            if (!hasError)
                File.Delete(tempLogFilePath);
            else zipLogFiles();
        }

        ~FileManager()
        {
            Dispose();
        }

        public string[] getInputFiles()
        {
            if (!Directory.Exists(inputPath))
                Directory.CreateDirectory(inputPath);

            List<string> inputDirectories = new List<string>(Directory.GetDirectories(inputPath));
            List<string> inputFiles = new List<string>(Directory.GetFiles(inputPath, "*.zip.*"));

            int inputFilePosition = 1;
            int inputFilesCount = inputFiles.Count;
            string decompressedInputFile, newDecompressedInputFile = string.Empty;

            for (int i = 0; i < inputFiles.Count; i++)
            {
                inputDirectories = new List<string>(Directory.GetDirectories(inputPath));

                if (inputFiles[i].ToLower().Contains(zip))
                {
                    string withoutExtension = Path.GetFileNameWithoutExtension(inputFiles[i]);
                    string withoutExtension2 = Path.GetFileNameWithoutExtension(withoutExtension);

                    if (!withoutExtension.Equals(withoutExtension2))
                        withoutExtension2 += Path.GetExtension(inputFiles[i]);

                    decompressedInputFile = Path.Combine(Directory.GetParent(inputFiles[i]).FullName, withoutExtension2);
                    newDecompressedInputFile = Path.Combine(Directory.GetParent(decompressedInputFile).FullName, FileManager.getUgoiraPixivNumber(decompressedInputFile));

                    if (Directory.Exists(newDecompressedInputFile))
                        Directory.Delete(newDecompressedInputFile, true);

                    if (inputFiles.Contains(newDecompressedInputFile))
                        inputFiles.Remove(newDecompressedInputFile);

                    if (inputDirectories.Contains(newDecompressedInputFile))
                        inputDirectories.Remove(newDecompressedInputFile);

                    if (Directory.Exists(decompressedInputFile))
                        Directory.Delete(decompressedInputFile, true);

                    onDecompressionStarted(decompressedInputFile, inputFilePosition, inputFilesCount);

                    ZipFile.ExtractToDirectory(inputFiles[i], decompressedInputFile, Encoding.UTF8);
                    inputFiles[i--] = decompressedInputFile;

                    onDecompressionPerformed(decompressedInputFile, inputFilePosition++, inputFilesCount);
                }
                else if (inputDirectories.Contains(inputFiles[i]))
                    inputFiles.RemoveAt(i--);
            }

            inputDirectories.AddRange(inputFiles);
            inputDirectories.Sort();

            return inputDirectories.ToArray();
        }

        public void scanErrorLog()
        {
            onLogFileScanningStarted();
            hasError = false;

            try
            {
                string[] prefixes = new string[] 
                {
                    "Saving file", 
                    "File", 
                    "Dimension", 
                    "Output", 
                    "Lossless", 
                    "* Header", 
                    "* Lossless", 
                    "* Precision", 
                    "* Palette size",
                    "PREDICTION",
                    "CROSS-COLOR-TRANSFORM",
                    "SUBTRACT-GREEN",
                    "PALETTE",
                    "Saved file",
                    "bytes used",
                    "line count",
                    "mode-partition",
                    "Residuals bytes",
                    "macroblocks",
                    "quantizer",
                    "filter level",
                    "block count",
                    "intra",
                    "skipped block",
                    "|"
                };

                StringReader reader = new StringReader(StandardStreamRedirector.getBuffer(StandardStreamRedirector.StreamFileDescriptor.Error));
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    bool err = true;

                    if (line.Trim().Length > 0)
                    {
                        foreach (string prefix in prefixes)
                        {
                            if (line.Trim().StartsWith(prefix))
                            {
                                err = false;
                                break;
                            }
                        }

                        if (err)
                        {
                            hasError = true;
                            break;
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                hasError = true;
            }
            finally
            {
                errorLogScanned = true;
                onLogFileScanningPerformed(hasError);
            }
        }
    }
}
