using CommonUtilities.StandardStreams.Redirectors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Pixiv.Utilities.Ugoira.Assembly.Managers
{
    public partial class FileManager
    {
        public static JArray integrityCheck(string ugoiraPath)
        {
            List<string> ugoiraFiles = new List<string>(Directory.GetFiles(ugoiraPath));
            List<int> ugoiraInfoIndexes = new List<int>();

            for (int i = 0; i < ugoiraFiles.Count; i++)
            {
                if (Path.GetFileName(ugoiraFiles[i] = ugoiraFiles[i]).ToLower().Equals(ugoiraInfo))
                    ugoiraInfoIndexes.Add(i);
            }

            if (ugoiraInfoIndexes.Count == 1)
            {
                ugoiraFiles.RemoveAt(ugoiraInfoIndexes[0]);

                JToken aniJson = JObject.Parse(File.ReadAllText(Path.Combine(ugoiraPath, ugoiraInfo)));
                JArray frames = (JArray)aniJson[ugokuInfo][ugokuFrameInfo];

                if (frames.Count == ugoiraFiles.Count)
                {
                    for (int i = 0; i < frames.Count; i++)
                    {
                        if (!frames[i][ugokuFrameFileInfo].ToString().Equals(Path.GetFileName(ugoiraFiles[i])))
                            return null;
                    }

                    return frames;
                }
            }
            return null;
        }

        public static string getUgoiraPixivNumber(string ugoiraPath)
        {
            StringBuilder newInputFileName = new StringBuilder();
            string newInputFile = string.Empty;

            foreach (char c in Path.GetFileNameWithoutExtension(ugoiraPath))
            {
                if (char.IsNumber(c))
                    newInputFileName.Append(c);
                else break;
            }

            if (newInputFileName.Length == 0)
                newInputFileName.Append(new Random().Next(100000000).ToString("D12"));

            return newInputFileName.ToString();
        }

        public static List<string> getConvertingOptions()
        {
            if (!File.Exists(configsFilePath))
                File.Create(configsFilePath).Close();

            List<string> options = new List<string>(File.ReadAllLines(configsFilePath));

            for (int i = 0; i < options.Count; i++)
            {
                if (isValidOption(options[i] = options[i].Trim()))
                {
                    if (options[i].Contains(space))
                    {
                        options.Insert(i + 1, options[i].Substring(options[i].IndexOf(space) + 1).Trim());
                        options[i] = options[i].Substring(0, options[i].IndexOf(space)).Trim();
                    }
                }
                else options.RemoveAt(i--);
            }

            return options;
        }

        private static bool isValidOption(string option)
        {
            int foo1 = 0;
            float foo2 = 0;
            option = option.Trim();

            return option.Equals("-lossless")
                || (option.StartsWith("-q ") && float.TryParse(option.Substring(3).Trim(), out foo2) && foo2 >= 0 && foo2 <= 100)
                || (option.StartsWith("-m ") && int.TryParse(option.Substring(3).Trim(), out foo1) && foo1 >= 0 && foo1 <= 6)
                || (float.TryParse(option, out foo2) && foo2 >= 0 && foo2 <= 100)
                || (int.TryParse(option, out foo1) && foo1 >= 0 && foo1 <= 6);
        }

        private static bool isFileLocked(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Write, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        private static void zipLogFiles()
        {
            if (Directory.Exists(logFilesDirectory))
                Directory.Delete(logFilesDirectory, true);
            Directory.CreateDirectory(logFilesDirectory);

            if (File.Exists(logZipFilePath))
            {
                ZipFile.ExtractToDirectory(logZipFilePath, tempProgressPath);
                File.Delete(logZipFilePath);
            }

            if (File.Exists(tempLogFilePath))
            {
                if (File.Exists(logFilePath))
                {
                    string logFileName = Path.GetFileNameWithoutExtension(logFilePath);

                    for (int i = 1; i <= int.MaxValue; i++)
                    {
                        string newLogFilePath = Path.Combine(tempProgressPath, logFileName + i + txt);

                        if (!File.Exists(newLogFilePath))
                        {
                            File.Move(tempLogFilePath, newLogFilePath);
                            break;
                        }
                    }
                }
                else File.Move(tempLogFilePath, logFilePath);
            }

            List<string> logFiles = new List<string>(Directory.GetFiles(tempProgressPath).Where<string>(e => Path.GetFileNameWithoutExtension(e).StartsWith(logFileNameWithoutExtension) && Path.GetFileName(e).EndsWith(txt)));
            foreach (string logFile in logFiles)
                File.Move(logFile, Path.Combine(logFilesDirectory, Path.GetFileName(logFile)));

            ZipFile.CreateFromDirectory(logFilesDirectory, logZipFilePath);
            Directory.Delete(logFilesDirectory, true);
        }
    }
}
