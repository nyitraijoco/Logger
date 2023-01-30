using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;

namespace LoggerLibrary
{
    public class Logger
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        public enum LoggerTypeEnum
        {
            console,
            file,
            stream
        }
        public enum LogLevelEnum
        {
            debug,
            info,
            error
        }
        public LoggerTypeEnum Type { get; set; }
        public string? FilePath { get; set; }
        public object? Stream { get; set; }
        public LogResult Result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerType">Where to save the log. Console or File or a Stream</param>
        /// <param name="arg">In case of Console it is not needed. File logtype it must be the full path of the file. In case of a Stream ot must be a Stream object.</param>
        public Logger(LoggerTypeEnum loggerType, object? arg = null)
        {
            Result = new();
            try
            {
                switch (loggerType)
                {
                    case LoggerTypeEnum.console:
                        AllocConsole();
                        break;
                    case LoggerTypeEnum.file:
                        if (arg == null) throw new Exception("arg cannot be null!");
                        if (arg.GetType() != typeof(string)) throw new Exception("arg must be String!");
                        FilePath = (string)arg;
                        break;
                    case LoggerTypeEnum.stream:
                        if (arg == null) throw new Exception("arg cannot be null!");
                        if (arg.GetType() != typeof(Stream) && arg.GetType().BaseType != typeof(Stream)) throw new Exception("arg must be a Stream!");
                        break;
                }
                Type = loggerType;

            } catch(Exception ex)
            {
                Result.Result = LogResult.ResultEnum.Error;
                Result.Exception = ex;
                
            }
        }
        public LogResult CreateLog(LogLevelEnum logLevel, string message)
        {
            try
            {
                switch (Type)
                {
                    case LoggerTypeEnum.console:
                        Result = LogConsole(logLevel, message);
                        break;
                    case LoggerTypeEnum.file:
                        Result = LogFile(logLevel, message);
                        break;
                    case LoggerTypeEnum.stream:
                        LogStream(logLevel, message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Result.Result = LogResult.ResultEnum.Error;
                Result.Exception = ex;
            }
            return Result;
        }
        private LogResult LogConsole(LogLevelEnum logLevel, string message)
        {
            try
            {
                if(message.Length > 1000) throw new Exception("Console cannot be longer than 1000 characters!");
                switch (logLevel)
                {
                    case LogLevelEnum.debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogLevelEnum.info:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogLevelEnum.error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.WriteLine(CreateLogString(logLevel,message));
            }catch(Exception ex)
            {
                Result.Exception = ex;
                Result.Result = LogResult.ResultEnum.Error;
            }
            finally
            {
                Console.ResetColor();
            }
            return Result;
        }
        private LogResult LogFile(LogLevelEnum logLevel, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath)) throw new Exception("FilePath is empty!");
                File.AppendAllText(FilePath, CreateLogString(logLevel, message) + Environment.NewLine);
                FileInfo fileInfo = new(FilePath);
                if (fileInfo.Length >= 5120)
                {
                    string filename = fileInfo.Name.Split('.', 2)[0];
                    string extension = fileInfo.Name.Split('.', 2)[1];
                    string[] fileList = Directory.GetFiles(fileInfo.DirectoryName, $"{filename}*{extension}");
                    int nextnum = 1;
                    if (fileList.Length > 1)
                    {
                        string lastlogfile = fileList.OrderByDescending(f => f).ToArray()[1];
                        int.TryParse(lastlogfile[^5].ToString(), out nextnum);
                        nextnum++;
                    }
                    File.Move(FilePath, $"{fileInfo.DirectoryName}\\{filename}.{nextnum}.{extension}");
                }
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
                Result.Result = LogResult.ResultEnum.Error;
            }
            return Result;
        }
        private LogResult LogStream(LogLevelEnum logLevel, string message)
        {
            LogResult result = new();
            try
            {
                if (Stream is null) throw new Exception("Stream is null!");
                byte[] bytes = Encoding.UTF8.GetBytes(CreateLogString(logLevel, message));
                Stream.GetType().GetMethods().Single(m => m.Name == "Write" && m.GetParameters().Length == 3).Invoke(Stream, new object[] { bytes, 0, bytes.Length });
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Result = LogResult.ResultEnum.Error;
            }
            return result;
        }
        private string CreateLogString(LogLevelEnum logLevel, string message)
        {
            //return string.Format("{0} [{1}] {2}", DateTime.Now, logLevel, message);
            return $"{DateTime.Now} [{logLevel}] {message}";
        }
    }
    public class LogResult
    {
        public enum ResultEnum
        {
            Success,
            Error
        }
        public ResultEnum Result { get; set; }
        public object? Exception { get; set; }
        public LogResult()
        {
            Result = ResultEnum.Success;
        }
    }
}