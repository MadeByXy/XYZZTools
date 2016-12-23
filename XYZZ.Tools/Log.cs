using System;
using System.IO;
using System.Text;

namespace XYZZ.Tools
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class Log
    {
        private static string fileName = "Log";

        private static FileInfo ErrorFile = SetFile();

        /// <summary>
        /// 文件名称
        /// </summary>
        public static string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                SetFile();
            }
        }

        private static FileInfo SetFile()
        {
            return new FileInfo(AppDomain.CurrentDomain.BaseDirectory + string.Format("{0}.txt", fileName));
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        public static void WriteLog(string text, params object[] args)
        {
            lock (ErrorFile)
            {
                FileStream fileStream;
                if (ErrorFile.CreationTime < DateTime.Now.AddDays(-10))
                {
                    ErrorFile.Delete();
                    fileStream = ErrorFile.Create();
                    ErrorFile.Refresh();
                }
                else
                {
                    fileStream = ErrorFile.Open(FileMode.Open);
                    new StreamReader(fileStream, Encoding.UTF8).ReadToEnd();
                }
                string originalText = string.Format(text, args) + "\r\n";
                byte[] bytes = Encoding.UTF8.GetBytes(originalText);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
                fileStream.Close();
            }
        }
    }
}
